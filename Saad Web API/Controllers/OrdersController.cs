using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Production;
using Saad_Web_API.Data;

namespace Saad_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : BasicController<Orders>
    {

        public OrdersController(ApplicationDbContext context) : base(context)
        {
        }

        // GET api/orders/search?{iscompleted}&{saleschannel}
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Orders>>> SearchOrders(
        [FromQuery] bool? isCompleted,
        [FromQuery] string? salesChannel)
        {
            var querry = _context.Orders.AsQueryable();
            if (isCompleted != null)
            {
                querry = querry.Where(c => c.IsCompleted == isCompleted);
            }
            if (!string.IsNullOrEmpty(salesChannel))
            {
                querry = querry.Where(c => c.SaleChannel != null && c.SaleChannel.Contains(salesChannel));
            }
            return Ok(await querry.ToListAsync());
        }

        //GET api/orders/{id}/products
        [HttpGet("{id}/products")]
        public async Task<ActionResult<IEnumerable<Products>>> GetOrderProducts(
            [FromRoute] int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            var products = await _context.Products.Where(o => o.Id == id).ToListAsync();
            return Ok(products);
        }
    }
}