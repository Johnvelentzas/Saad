using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Attributes;
using Models.Management;
using Models.Production;
using Saad_Web_API.Data;

namespace Saad_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : BasicController<Tasks>
    {

        public TasksController(ApplicationDbContext context) : base(context)
        {
        }

        //GET api/tasks/{id}/process
        [HttpGet("{id}/process")]
        public async Task<ActionResult<Processes>> GetTaskProcess(
            [FromRoute] int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            var process = await _context.Processes.FindAsync(task.ProcessId);
            return Ok(process);
        }

        //Get api/tasks/filter?{iscomplete}&{active}
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Tasks>>> GetFilteredTasks(
            [FromQuery] bool? isComplete = null,
            [FromQuery] bool? active = null)
        {
            var query = _context.Tasks.AsQueryable();
            if (isComplete != null)
            {
                query = query.Where(t => t.IsCompleted == isComplete);
            }
            if (active != null)
            {
                var activeTasks = GetActiveTasks(_context).Result;
                query = query.Where(t => activeTasks.Where(o => t.Id == o.Id).Any());
            }
            var tasks = await query.ToListAsync();
            return Ok(tasks);
        }

        public static async Task<IEnumerable<Tasks>> GetActiveTasks(ApplicationDbContext _context)
        {
            var incompleteTasks = await _context.Tasks.Where(t => !t.IsCompleted).ToListAsync();
            var inactiveDependencies = await _context.TaskDependencies.Where(o => incompleteTasks.Where(t => o.DependsOnTaskId == t.Id).Any()).ToListAsync();
            var activeTasks = await _context.Tasks.Where(t => !inactiveDependencies.Where(d => t.Id == d.TaskId).Any()).ToListAsync();

            return activeTasks;
        }

        [HttpGet("available")]
        public async Task<ActionResult<List<Tasks>>> GetAvailableTasks([FromQuery] List<int> processIds)
        {
            var query = _context.Tasks.AsQueryable();

            // 1. Filter by the list of processes (Translates to SQL: WHERE ProcessId IN (x, y, z))
            if (processIds != null && processIds.Any())
            {
                query = query.Where(t => processIds.Contains(t.ProcessId));
            }

            // 2. The Core Logic: Not completed, and NO incomplete prerequisites
            var availableTasks = await query
                .Where(t => t.IsCompleted == false)
                .Where(t => t.IsCancelled == false)
                .Where(t => !_context.TaskDependencies
                    .Where(td => td.TaskId == t.Id)
                    .Join(_context.Tasks,
                          td => td.DependsOnTaskId,
                          prereq => prereq.Id,
                          (td, prereq) => prereq)
                    .Any(prereq => prereq.IsCompleted == false))
                .OrderBy(t => t.FinishBy)
                .ToListAsync();

            return Ok(availableTasks);
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteTask([FromRoute] int id, [FromQuery] int userId)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null) return NotFound("Task not found.");
            if (task.IsCompleted) return BadRequest("Task is already completed.");
            if (userId <= 0) return BadRequest("A valid UserId is required to complete a task.");

            // Update the task state
            task.IsCompleted = true;
            task.UserId = userId; // Record exactly who did the work!

            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok("Task completed successfully.");
        }
    }
}