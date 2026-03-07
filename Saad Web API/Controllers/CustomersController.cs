using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Finances;
using Models.Production;
using Saad_Web_API.Data;

namespace Saad_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : BasicController<Customers>
    {

        public CustomersController(ApplicationDbContext context) : base(context)
        {
        }
        // GET api/customers/search?{name}&{tn}&{email}&{telephone}
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Customers>>> SearchCustomers(
            [FromQuery] string? name,
            [FromQuery] string? TN,
            [FromQuery] string? email,
            [FromQuery] string? telephone)
        {
            var querry = _context.Customers.AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                querry = querry.Where(c => (c.FirstName != null && c.FirstName.Contains(name)) || c.LastName.Contains(name));
            }
            if (!string.IsNullOrEmpty(TN))
            {
                querry = querry.Where(c => c.TaxNumber != null && c.TaxNumber.Contains(TN));
            }
            if (!string.IsNullOrEmpty(email))
            {
                querry = querry.Where(c => c.Email != null && c.Email.Contains(email.ToLower()));
            }
            if (!string.IsNullOrEmpty(telephone))
            {
                querry = querry.Where(c => c.Telephone != null && c.Telephone.Contains(telephone));
            }
            return Ok(await querry.ToListAsync());
        }

        //GET api/customers/{id}/orders
        [HttpGet("{id}/orders")]
        public async Task<ActionResult<IEnumerable<Orders>>> GetCustomerOrders(
            [FromRoute] int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            var orders = await _context.Orders.Where(o => o.CustomerId == id).ToListAsync();
            return Ok(orders);
        }
    }
}
