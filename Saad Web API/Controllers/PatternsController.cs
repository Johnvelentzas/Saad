using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Attributes;
using Saad_Web_API.Data;

namespace Saad_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatternsController : BasicController<Patterns>
    {
        public PatternsController(ApplicationDbContext context) : base(context)
        {
        }


        //GET api/models/{id}/patterns
        [HttpGet("~/api/models/{id}/patterns")]
        public async Task<ActionResult<IEnumerable<Patterns>>> GetPatternsFromModel(
            [FromRoute] int id,
            [FromQuery] List<FilterType> filters,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] SortType sort = SortType.IdDecending)
        {
            var model = await _context.Models.FindAsync(id);
            if (model == null)
            {
                return NotFound("Model doesn't exist");
            }
            IQueryable<Patterns> query = await GetQuery<Patterns>();
            query = await OrderQuery(query, sort);
            query = await FilterEntities(query, filters);
            query = query.Where(o => o.ModelId == id);
            var pageResult = await Paginate(query, page, pageSize);
            return Ok(pageResult);

        }
    }
}