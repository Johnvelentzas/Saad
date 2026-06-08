using Models.Attributes;
using Models.Management;
using Models.Production;

namespace Saad_Web_API.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            bool saveRequired = false;

            // --- 1. HANDLE PROCESSES & AESTHETICS ---
            var allProcessTypes = Enum.GetValues<ProcessesType>().ToList();
            var existingProcesses = context.Processes.ToList();
            var existingProcessTypes = existingProcesses.Select(p => p.Type).ToList();

            // A. Add Missing Processes (with colors injected immediately!)
            var missingProcessTypes = allProcessTypes.Except(existingProcessTypes).ToList();
            if (missingProcessTypes.Any())
            {
                foreach (var type in missingProcessTypes)
                {
                    var newProcess = new Processes { Type = type };
                    ApplyProcessAesthetics(newProcess); // Apply colors BEFORE adding to database
                    context.Processes.Add(newProcess);
                    existingProcesses.Add(newProcess);  // Add to local list so Admin logic sees it
                }
                saveRequired = true;
            }

            // B. Fix Existing Processes that are missing colors
            foreach (var process in existingProcesses)
            {
                // IsNullOrWhiteSpace guarantees it catches empty spaces from the migration
                if (string.IsNullOrWhiteSpace(process.IconText) || string.IsNullOrWhiteSpace(process.Color))
                {
                    ApplyProcessAesthetics(process);
                    // No UpdateRange needed! The EF Change Tracker automatically knows we changed the string.
                    saveRequired = true;
                }
            }

            // Save all process changes in one go
            if (saveRequired)
            {
                context.SaveChanges();
                saveRequired = false; // Reset flag for the users section
            }

            // --- 2. HANDLE ADMIN USER ---
            var admin = context.Users.FirstOrDefault(u => u.Name == "Admin");
            if (admin == null)
            {
                admin = new Models.Management.Users
                {
                    Name = "Admin",
                };

                context.Users.Add(admin);
                context.SaveChanges(); // Save immediately to generate the new admin.Id
            }

            // --- 3. HANDLE ADMIN PERMISSIONS ---
            // Get all process IDs directly from the database to ensure we have the newly generated IDs
            var allProcessIds = context.Processes.Select(p => p.Id).ToList();

            var adminExistingProcessIds = context.UserProcesses
                .Where(up => up.UserId == admin.Id)
                .Select(up => up.ProcessId)
                .ToList();

            var missingProcessIds = allProcessIds.Except(adminExistingProcessIds).ToList();

            if (missingProcessIds.Any())
            {
                var adminProcessesToAdd = missingProcessIds.Select(processId => new Models.Management.UserProcesses
                {
                    UserId = admin.Id,
                    ProcessId = processId
                }).ToList();

                context.UserProcesses.AddRange(adminProcessesToAdd);
                context.SaveChanges();
            }
        }

        private static void ApplyProcessAesthetics(Processes process)
        {
            switch (process.Type)
            {
                // Production (Soft Royal Blue)
                case ProcessesType.Customers: process.IconText = "\ue7fb"; process.Color = "#5C93FA"; break;
                case ProcessesType.Orders: process.IconText = "\uef71"; process.Color = "#5C93FA"; break;
                case ProcessesType.Products: process.IconText = "\ue1a1"; process.Color = "#5C93FA"; break;

                // Management (Periwinkle)
                case ProcessesType.Users: process.IconText = "\uf033"; process.Color = "#8C9EFF"; break;

                // Attributes (Lilacs & Plums)
                case ProcessesType.ProductCategories: process.IconText = "\ue574"; process.Color = "#B39DDB"; break;
                case ProcessesType.Brands: process.IconText = "\ue023"; process.Color = "#CE93D8"; break;
                case ProcessesType.Models: process.IconText = "\ue9f9"; process.Color = "#B39DDB"; break;
                case ProcessesType.Patterns: process.IconText = "\uf025"; process.Color = "#CE93D8"; break;
                case ProcessesType.StitchTypes: process.IconText = "\ue176"; process.Color = "#B39DDB"; break;
                case ProcessesType.YarnColors: process.IconText = "\ue40a"; process.Color = "#CE93D8"; break;
                case ProcessesType.Fabrics: process.IconText = "\ue421"; process.Color = "#B39DDB"; break;

                // Appointments (Mint Green)
                case ProcessesType.DropOffApt: process.IconText = "\ue168"; process.Color = "#66BB6A"; break;
                case ProcessesType.TestTryApt: process.IconText = "\uf1c2"; process.Color = "#66BB6A"; break;
                case ProcessesType.PickUpApt: process.IconText = "\ue558"; process.Color = "#66BB6A"; break;

                // Foam Tasks (Bright Cyan)
                case ProcessesType.FoamFix: process.IconText = "\uf100"; process.Color = "#26C6DA"; break;
                case ProcessesType.FoamAdapt: process.IconText = "\ue428"; process.Color = "#26C6DA"; break;
                case ProcessesType.FoamGel: process.IconText = "\ue798"; process.Color = "#26C6DA"; break;
                case ProcessesType.FoamAnatomical: process.IconText = "\ue92c"; process.Color = "#26C6DA"; break;

                // Tasks (Warm Coral & Orange)
                case ProcessesType.CoverRemove: process.IconText = "\ue53b"; process.Color = "#FF8A65"; break;
                case ProcessesType.CustomPattern: process.IconText = "\uf10a"; process.Color = "#FFB74D"; break;
                case ProcessesType.Cut: process.IconText = "\ue14e"; process.Color = "#FF8A65"; break;
                case ProcessesType.Sew: process.IconText = "\ue155"; process.Color = "#FFB74D"; break;
                case ProcessesType.Embroider: process.IconText = "\ue65f"; process.Color = "#FF8A65"; break;
                case ProcessesType.Bolt: process.IconText = "\uea59"; process.Color = "#FFB74D"; break;
                case ProcessesType.Inspect: process.IconText = "\uf0c5"; process.Color = "#FF8A65"; break;

                // Tabs (Deep Slate)
                case ProcessesType.Tasks: process.IconText = "\uf045"; process.Color = "#455A64"; break;
                case ProcessesType.Foam: process.IconText = "\ue2bd"; process.Color = "#455A64"; break;
                case ProcessesType.Calendar: process.IconText = "\uebcc"; process.Color = "#455A64"; break;
            }
        }
    }
}