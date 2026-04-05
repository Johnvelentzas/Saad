using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
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
    }
}