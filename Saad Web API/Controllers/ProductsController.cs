using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Production;
using Saad_Web_API.Data;

namespace Saad_Web_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Products>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        //GET api/Products/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Products>> GetProductById(
            [FromRoute]int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        //POST api/Products
        [HttpPost]
        public async Task<ActionResult<Products>> CreateProduct(
            [FromBody] Products product)
        {
            var newProduct = new Products
            {
                OrderId = product.OrderId,
                CategoryId = product.CategoryId,
                ModelId = product.ModelId,
                IsCompleted = product.IsCompleted,
                CreatedDate = product.CreatedDate,
                ExpectedStartDate = product.ExpectedStartDate,
                ExpectedFinishDate = product.ExpectedFinishDate,
                Comments = product.Comments
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
        }

        //PUT api/Products/id
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(
            [FromRoute]int id,
            [FromBody] Products product)
        {
            if (id != product.Id)
            {
                return BadRequest("The id provided doesnt match the Product id");
            }
            _context.Entry(product).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                throw;
            }
            return NoContent();
        }

        //DELETE api/Products/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(
            [FromRoute]int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        //GET api/Products/search?orderId=1&categoryId=2
        //TODO : Implement the logic to search a prodcut by varius parameters and return the matching products as a response.
    }
}
