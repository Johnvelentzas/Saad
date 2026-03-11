using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Production;
using Saad_Web_API.Data;

namespace Saad_Web_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : BasicController<Products>
    {
        public ProductsController(ApplicationDbContext context) : base(context)
        {
        }

        // GET api/products/search?
        // {iscomplete}&
        // {createdafter}&
        // {createdbefore}&
        // {expectedstartdatelow}&
        // {expectedstartdatehigh}&
        // {expectedfinishdatelow}&
        // {expectedfinishdatehigh}
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Products>>> SearchProducts(
            [FromQuery] bool? isComplete,
            [FromQuery] DateTime? createdAfter,
            [FromQuery] DateTime? createdBefore,
            [FromQuery] DateTime? expectedStartDateLow,
            [FromQuery] DateTime? expectedStartDateHigh,
            [FromQuery] DateTime? expectedFinishDateLow,
            [FromQuery] DateTime? expectedFinishDateHigh)
        {
            var querry = _context.Products.AsQueryable();
            if (isComplete != null)
            {
                querry = querry.Where(c => c.IsCompleted == isComplete);
            }
            if (createdBefore != null)
            {
                querry = querry.Where(c => c.CreatedDate < createdBefore);
            }
            if (createdAfter != null)
            {
                querry = querry.Where(c => c.CreatedDate > createdAfter);
            }
            if (expectedStartDateHigh != null)
            {
                querry = querry.Where(c => c.ExpectedStartDate < expectedStartDateHigh);
            }
            if (expectedStartDateLow != null)
            {
                querry = querry.Where(c => c.ExpectedStartDate > expectedStartDateLow);
            }
            if (expectedFinishDateHigh != null)
            {
                querry = querry.Where(c => c.ExpectedFinishDate < expectedFinishDateHigh);
            }
            if (expectedFinishDateLow != null)
            {
                querry = querry.Where(c => c.ExpectedFinishDate > expectedFinishDateLow);
            }

            return Ok(await querry.ToListAsync());
        }

        //GET api/products/{id}/tasks
        [HttpGet("{id}/tasks")]
        public async Task<ActionResult<IEnumerable<Tasks>>> GetProductTasks(
            [FromRoute] int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var tasks = await _context.Tasks.Where(o => o.ProductId == id).ToListAsync();
            return Ok(Tasks);
        }
    }
}
