using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Attributes;
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

        //GET api/tasks/{id}/attributes
        [HttpGet("{id}/attributes")]
        public async Task<ActionResult<IEnumerable<AttributeValues>>> GetTaskAttributes(
            [FromRoute] int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            var attributes = await _context.TaskAtributes.Where(a => a.TaskId == id).ToListAsync();
            var attributeValues = await _context.AttributeValues.Where(a => attributes.Where(o => o.AttributeId == a.Id).Any()).ToListAsync();
            return Ok(attributeValues);
        }

        public static async Task<IEnumerable<Tasks>> GetActiveTasks(ApplicationDbContext _context)
        {
            var incompleteTasks = await _context.Tasks.Where(t => !t.IsCompleted).ToListAsync();
            var inactiveDependencies = await _context.TaskDependencies.Where(o => incompleteTasks.Where(t => o.DependsOnTaskId == t.Id).Any()).ToListAsync();
            var activeTasks = await _context.Tasks.Where(t => !inactiveDependencies.Where(d => t.Id == d.TaskId).Any()).ToListAsync();

            return activeTasks;
        }
    }
}