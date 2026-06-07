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

        //GET api/brands/{id}/models
        [HttpGet("~/api/brands/{id}/models")]
        public async Task<ActionResult<IEnumerable<Models.Attributes.Models>>> GetModelsFromBrand(
            [FromRoute] int id,
            [FromQuery] List<FilterType> filters,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] SortType sort = SortType.IdDecending)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound("Brand doesn't exist");
            }
            IQueryable<Models.Attributes.Models> query = await GetQuery<Models.Attributes.Models>();
            query = await OrderQuery(query, sort);
            query = await FilterEntities(query, filters);
            query = query.Where(o => o.BrandId == id);
            var pageResult = await Paginate(query, page, pageSize);
            return Ok(pageResult);
        }

        // GET api/models/search
        [HttpGet("search")]
        public async Task<ActionResult<RequestResult<Models.Attributes.Models>>> SearchModels(
            [FromQuery] List<FilterType> filters,
            [FromQuery] int? brandId = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] SearchType searchType = SearchType.General,
            [FromQuery] string searchValue = "",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] SortType sort = SortType.IdDecending)
        {
            IQueryable<Models.Attributes.Models> query = await GetQuery<Models.Attributes.Models>();
            if (brandId.HasValue)
            {
                query = query.Where(m => m.BrandId == brandId.Value);
            }
            if (categoryId.HasValue)
            {
                query = query.Where(m => m.CategoryId == categoryId.Value);
            }
            query = await FilterEntities(query, filters);
            query = await SearchEntities(query, searchType, searchValue.ToLower());
            query = await OrderQuery(query, sort);
            var pageResult = await Paginate(query, page, pageSize);
            return Ok(pageResult);
        }
    }
}