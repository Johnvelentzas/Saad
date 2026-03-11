using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Attributes;
using Saad_Web_API.Data;

namespace Saad_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : BasicController<ProductCategories>
    {
        public CategoriesController(ApplicationDbContext context) : base(context)
        {
        }

        //GET api/categories/{id}/models
        [HttpGet("{id}/models")]
        public async Task<ActionResult<IEnumerable<Models.Attributes.Models>>> GetModelsFromCategory(
            [FromRoute] int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound("Category doesn't exist");
            }
            var models = await _context.Models.Where(o => o.CategoryId == id).ToListAsync();
            return Ok(models);
        }
    }
}
