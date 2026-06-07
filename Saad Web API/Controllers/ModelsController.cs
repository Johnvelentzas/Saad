using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Attributes;
using Models.Production;
using Saad_Web_API.Data;

namespace Saad_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelsController : BasicController<Models.Attributes.Models>
    {
        public ModelsController(ApplicationDbContext context) : base(context)
        {
        }

        //GET api/categories/{id}/models
        [HttpGet("~/api/categories/{id}/models")]
        public async Task<ActionResult<IEnumerable<Models.Attributes.Models>>> GetModelsFromCategory(
            [FromRoute] int id,
            [FromQuery] List<FilterType> filters,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] SortType sort = SortType.IdDecending)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound("Category doesn't exist");
            }
            IQueryable<Models.Attributes.Models> query = await GetQuery<Models.Attributes.Models>();
            query = await OrderQuery(query, sort);
            query = await FilterEntities(query, filters);
            query = query.Where(o => o.CategoryId == id);
            var pageResult = await Paginate(query, page, pageSize);
            return Ok(pageResult);
        }
    }
}