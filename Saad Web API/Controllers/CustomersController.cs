using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Finances;
using Saad_Web_API.Data;

namespace Saad_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customers>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        // GET api/Customers/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Customers>> GetCustomerById(
            [FromRoute]int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer); 
        }

        // PUT api/Customers/id
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(
            [FromRoute]int id, 
            [FromBody] Customers customer)
        {
            if (id != customer.Id)
                {
                    return BadRequest("The id provided doesnt match the Customer id");
                }
    
            _context.Entry(customer).State = EntityState.Modified;

            try
             {
                 await _context.SaveChangesAsync();
             }
             catch (DbUpdateConcurrencyException)
             {
                 if (!_context.Customers.Any(e => e.Id == id))
                 {
                     return NotFound();
                 }
                 throw;
                }
            return NoContent();
        }

        // POST api/Customers
        [HttpPost]
        public async Task<ActionResult<Customers>> CreateCustomer(
            [FromBody] Customers customer)
        {
            var newCustomer = new Customers
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Telephone = customer.Telephone,
                TaxNumber = customer.TaxNumber,
                CreatedDate = DateTime.Today
            };

            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomerById), new { id = newCustomer.Id }, newCustomer);
        }

        // DELETE api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(
            [FromRoute]int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET api/Customers/search?name=&TN=&email=&telephone=
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
    }
}
