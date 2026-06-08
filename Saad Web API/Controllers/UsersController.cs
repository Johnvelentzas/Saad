using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Management;
using Saad_Web_API.Data;

namespace Saad_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BasicController<Users>
    {
        public UsersController(ApplicationDbContext context) : base(context)
        {
        }

        //GET api/users/{id}/processes
        [HttpGet("{id}/processes")]
        public async Task<ActionResult<RequestResult<Processes>>> GetUserProcesses(
            [FromRoute] int id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] SortType sort = SortType.IdDecending)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            IQueryable<Processes> processes = await GetQuery<Processes>();
            IQueryable<UserProcesses> userProcesses = await GetQuery<UserProcesses>();
            var result = processes.Where(p => userProcesses.Where(up => up.UserId == user.Id && up.ProcessId == p.Id).Any());
            var pageResult = await Paginate(result, page, pageSize);
            return Ok(pageResult);
        }

        [HttpPost("{userId}/processes/{processId}")]
        public async Task<IActionResult> EnableProcess(
            [FromRoute] int userId, 
            [FromRoute] int processId)
        {
            var existingLink = await _context.UserProcesses.FirstOrDefaultAsync(up => up.UserId == userId && up.ProcessId == processId);
            if(existingLink != null) { return Ok(); }
            _context.UserProcesses.Add(new UserProcesses { UserId = userId, ProcessId = processId });

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{userId}/processes/{processId}")]
        public async Task<IActionResult> DisableProcess(
            [FromRoute] int userId,
            [FromRoute] int processId)
        {
            if (userId == 1) { return Conflict("Cannot modify permissions for the admin user."); }
            var existingLink = await _context.UserProcesses.FirstOrDefaultAsync(up => up.UserId == userId && up.ProcessId == processId);
            if (existingLink == null) { return Ok(); }
            _context.UserProcesses.Remove(existingLink);

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public override async Task<IActionResult> Delete(
            [FromRoute] int id)
        {
            if (id == 1)
            {
                return Conflict("The admin account cannot be deleted.");
            }
            return await base.Delete(id);
        }
    }
}