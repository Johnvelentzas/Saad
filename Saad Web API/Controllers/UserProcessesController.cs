using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Management;
using Models.Production;
using Saad_Web_API.Data;

namespace Saad_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProcessesController : BasicController<UserProcesses>
    {
        public UserProcessesController(ApplicationDbContext context) : base(context)
        {
        }

        [HttpGet("~/api/users/{id}/userprocesses")]
        public async Task<ActionResult<RequestResult<UserProcesses>>> GetUserProcesses(
            [FromRoute] int id,
            [FromQuery] List<FilterType> filters,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] SortType sort = SortType.IdDecending)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            IQueryable<UserProcesses> query = await GetQuery<UserProcesses>();
            query = await OrderQuery(query, sort);
            query = await FilterEntities(query, filters);
            query = query.Where(o => o.UserId == id);
            var pageResult = await Paginate(query, page, pageSize);
            return Ok(pageResult);
        }
    }
}