using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Finances;
using Models.Production;
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

        //GET api/categories/{id}/products
        [HttpGet("{id}/products")]
        public async Task<ActionResult<IEnumerable<Products>>> GetCategoryProducts(
            [FromRoute] int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var products = await _context.Products.Where(o => o.CategoryId == id).ToListAsync();
            return Ok(products);
        }
    }
}
