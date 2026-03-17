using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Attributes;
using Models.Production;
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

        //GET api/users/search?{name}
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Users>>> SearchUsers(
            [FromQuery] string? name)
        {
            var querry = _context.Users.AsQueryable();
            if (name != null)
            {
                querry.Where(o => o.Name == name);
            }
            return Ok(await querry.ToListAsync());
        }

        //GET api/users/{id}/processes
        [HttpGet("{id}/processes")]
        public async Task<ActionResult<IEnumerable<Processes>>> GetUserProcesses(
            [FromRoute] int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var processes = await _context.Processes.Where(p => _context.UserProcesses.Where(up => up.UserId == user.Id && up.ProcessId == p.Id).Any()).ToListAsync();
            return Ok(processes);
        }
    }
}