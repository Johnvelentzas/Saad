using Microsoft.EntityFrameworkCore;
using Models.Management;
using Models.Production;
using Saad_Web_API.Data;

namespace Saad_Web_API.Helpers
{
    public interface IProductionWorkflowService
    {
        Task SyncTasksForProduct(Products product);
        Task PauseProductionForProduct(Products product);
    }

    public class ProductionWorkflowService : IProductionWorkflowService
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductionWorkflowService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SyncTasksForProduct(Products product)
        {

            // 0. Flip the state variable to true
            product.HasStartedManufacturing = true;
            _dbContext.Entry(product).State = EntityState.Modified;


            // 1. Get Process IDs map
            var processMap = await _dbContext.Processes.ToDictionaryAsync(p => p.Type, p => p.Id);
            int GetProcessId(ProcessesType type) => processMap.TryGetValue(type, out var id) ? id : 0;

            // 2. Fetch existing tasks
            var existingTasks = await _dbContext.Tasks
                .Where(t => t.ProductId == product.Id)
                .ToListAsync();

            var activeTasks = new HashSet<Tasks>();
            var tasksToSave = new List<Tasks>();

            // --- DATE CALCULATION LOGIC (Backward Scheduling) ---

            DateTime SubtractWorkingDays(DateTime date, int daysToSubtract)
            {
                // Load holidays for the current year. 
                // If we are in January, load last year's holidays too, just in case we subtract past Jan 1st.
                var holidays = GreekHolidayCalculator.GetHolidaysForYear(date.Year);
                if (date.Month == 1)
                {
                    holidays.UnionWith(GreekHolidayCalculator.GetHolidaysForYear(date.Year - 1));
                }

                DateTime result = date;

                while (daysToSubtract > 0)
                {
                    result = result.AddDays(-1);

                    // Check 1: Is it the weekend? (Saturday or Sunday)
                    bool isWeekend = result.DayOfWeek == DayOfWeek.Saturday ||
                                     result.DayOfWeek == DayOfWeek.Sunday;

                    // Check 2: Is it a Greek public holiday?
                    bool isHoliday = holidays.Contains(result.Date);

                    // If it is a normal working day, we successfully consumed one of the subtraction days.
                    if (!isWeekend && !isHoliday)
                    {
                        daysToSubtract--;
                    }
                }

                return result;
            }

            // Phase 3: Assembly (Anchored to final deadline)
            DateTime assemblyDate = product.ExpectedFinishDate ?? DateTime.Today.AddDays(15);

            // Phase 2: Pattern & Fabric 
            DateTime patternDate = product.HasCustomPatternTask ? SubtractWorkingDays(assemblyDate, 1) : assemblyDate;

            // Phase 1: Prep & Foam (Anchored to Test Try if it exists)
            DateTime prepDate = product.HasTestTryApt && product.TestTryApt.HasValue
                ? product.TestTryApt.Value.Date
                : SubtractWorkingDays(patternDate, 1);

            // Override: Late Drop Off pushes the schedule forward
            if (product.HasDropOffApt && product.DropOffApt.HasValue && product.DropOffApt.Value.Date > prepDate)
            {
                prepDate = product.DropOffApt.Value.Date;
                if (patternDate < prepDate) patternDate = prepDate;
                if (assemblyDate < patternDate) assemblyDate = patternDate;
            }

            // --- THE UNIFIED HELPER ---
            Tasks GetOrCreateTask(ProcessesType type, DateTime targetDate)
            {
                int pId = GetProcessId(type);
                if (pId == 0) return null;

                var existing = existingTasks.FirstOrDefault(t => t.ProcessId == pId);
                if (existing != null)
                {
                    activeTasks.Add(existing);
                    if (existing.IsCancelled) existing.IsCancelled = false; // Un-cancel if revived
                    existing.FinishBy = targetDate; // Always update to latest calculated deadline
                    return existing;
                }

                var newTask = new Tasks
                {
                    ProductId = product.Id,
                    ProcessId = pId,
                    UserId = 0,
                    IsCompleted = false,
                    IsCancelled = false,
                    IsDraft = false,
                    FromId = 0,
                    FinishBy = targetDate, // Assign the calculated phase date
                    CreatedDate = DateTime.Now
                };
                tasksToSave.Add(newTask);
                activeTasks.Add(newTask);
                return newTask;
            }

            var dependencyMap = new List<(Tasks Dependent, Tasks Prerequisite)>();
            Tasks latestSeatTask = null;
            Tasks latestCoverTask = null;

            // --- 1. SEAT CHAIN (Phase 1: prepDate) ---

            if (product.HasDropOffApt)
            {
                latestSeatTask = GetOrCreateTask(ProcessesType.DropOffApt, product.DropOffApt ?? prepDate);
            }

            if (product.HasRipTask)
            {
                var rip = GetOrCreateTask(ProcessesType.CoverRemove, prepDate);
                if (rip != null && latestSeatTask != null) dependencyMap.Add((rip, latestSeatTask));
                latestSeatTask = rip ?? latestSeatTask;
            }

            if (product.FoamType != FoamType.None)
            {
                ProcessesType foamProcess = product.FoamType switch
                {
                    FoamType.FoamFix => ProcessesType.FoamFix,
                    FoamType.FoamAdapt => ProcessesType.FoamAdapt,
                    FoamType.FoamAnatomical => ProcessesType.FoamAnatomical,
                    _ => ProcessesType.FoamFix
                };
                var foam = GetOrCreateTask(foamProcess, prepDate);
                if (foam != null && latestSeatTask != null) dependencyMap.Add((foam, latestSeatTask));
                latestSeatTask = foam ?? latestSeatTask;
            }

            if (product.HasGelTask)
            {
                var gel = GetOrCreateTask(ProcessesType.FoamGel, prepDate);
                if (gel != null && latestSeatTask != null) dependencyMap.Add((gel, latestSeatTask));
                latestSeatTask = gel ?? latestSeatTask;
            }

            if (product.HasTestTryApt)
            {
                var testTry = GetOrCreateTask(ProcessesType.TestTryApt, prepDate);
                if (testTry != null && latestSeatTask != null) dependencyMap.Add((testTry, latestSeatTask));
                latestSeatTask = testTry ?? latestSeatTask;
            }

            // --- 2. COVER CHAIN (Phase 2: patternDate) ---

            if (product.HasCustomPatternTask)
            {
                var pattern = GetOrCreateTask(ProcessesType.CustomPattern, patternDate);
                if (pattern != null && latestSeatTask != null) dependencyMap.Add((pattern, latestSeatTask));
                latestCoverTask = pattern;
            }

            var cut = GetOrCreateTask(ProcessesType.Cut, patternDate);
            if (cut != null && latestCoverTask != null) dependencyMap.Add((cut, latestCoverTask));
            latestCoverTask = cut ?? latestCoverTask;

            if (product.HasEmbroideryTask)
            {
                var embroidery = GetOrCreateTask(ProcessesType.Embroider, patternDate);
                if (embroidery != null && latestCoverTask != null) dependencyMap.Add((embroidery, latestCoverTask));
                latestCoverTask = embroidery ?? latestCoverTask;
            }

            // --- 3. CONVERGENCE / ASSEMBLY (Phase 3: assemblyDate) ---

            var sew = GetOrCreateTask(ProcessesType.Sew, assemblyDate);
            if (sew != null && latestCoverTask != null) dependencyMap.Add((sew, latestCoverTask));
            latestCoverTask = sew ?? latestCoverTask;

            Tasks latestAssemblyTask = latestCoverTask;

            if (product.HasBoltTask)
            {
                var bolt = GetOrCreateTask(ProcessesType.Bolt, assemblyDate);
                if (bolt != null)
                {
                    if (latestCoverTask != null) dependencyMap.Add((bolt, latestCoverTask));
                    if (latestSeatTask != null) dependencyMap.Add((bolt, latestSeatTask));
                }
                latestAssemblyTask = bolt ?? latestAssemblyTask;
            }

            var inspect = GetOrCreateTask(ProcessesType.Inspect, assemblyDate);
            if (inspect != null && latestAssemblyTask != null) dependencyMap.Add((inspect, latestAssemblyTask));

            if (product.HasPickUpApt)
            {
                var pickup = GetOrCreateTask(ProcessesType.PickUpApt, product.PickUpApt ?? assemblyDate);
                if (pickup != null && inspect != null) dependencyMap.Add((pickup, inspect));
            }

            // --- 4. EXECUTE DB SYNC ---

            // A. Execute Soft-Cancels for abandoned tasks
            var abandonedTasks = existingTasks.Except(activeTasks).ToList();
            foreach (var abandoned in abandonedTasks)
            {
                abandoned.IsCancelled = true;
            }

            // B. Save brand new tasks
            if (tasksToSave.Any())
            {
                _dbContext.Tasks.AddRange(tasksToSave);
                await _dbContext.SaveChangesAsync();
            }

            // C. Wipe old dependencies
            var allProductTaskIds = existingTasks.Select(t => t.Id).Concat(tasksToSave.Select(t => t.Id)).ToList();
            var oldDependencies = await _dbContext.TaskDependencies
                .Where(td => allProductTaskIds.Contains(td.TaskId))
                .ToListAsync();

            _dbContext.TaskDependencies.RemoveRange(oldDependencies);

            // D. Map and save new dependencies
            var dependenciesToSave = dependencyMap
                .Where(map => map.Dependent?.Id > 0 && map.Prerequisite?.Id > 0)
                .Select(map => new TaskDependencies
                {
                    TaskId = map.Dependent.Id,
                    DependsOnTaskId = map.Prerequisite.Id,
                    IsDraft = false,
                    FromId = 0,
                    CreatedDate = DateTime.Now
                }).ToList();

            _dbContext.TaskDependencies.AddRange(dependenciesToSave);
            await _dbContext.SaveChangesAsync();
        }

        public async Task PauseProductionForProduct(Products product)
        {
            // 1. Revert the state variable
            product.HasStartedManufacturing = false;
            _dbContext.Entry(product).State = EntityState.Modified;

            // 2. Find all tasks that haven't been finished yet
            var pendingTasks = await _dbContext.Tasks
                .Where(t => t.ProductId == product.Id && t.IsCompleted == false)
                .ToListAsync();

            // 3. Cancel them to clear them from the workers' tablets
            foreach (var task in pendingTasks)
            {
                task.IsCancelled = true;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}