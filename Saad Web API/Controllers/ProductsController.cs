using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Production;
using Saad_Web_API.Data;
using Saad_Web_API.Helpers;

namespace Saad_Web_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : BasicController<Products>
    {

        private readonly IProductionWorkflowService _workflowService;
        public ProductsController(ApplicationDbContext context, IProductionWorkflowService workflowService) : base(context)
        {
            _workflowService = workflowService;
        }

        protected override async Task<IQueryable<Products>> SearchEntities(IQueryable<Products> products, SearchType type, string value)
        {
            products = await base.SearchEntities(products, type, value);
            switch (type)
            {
                case SearchType.General:
                    products = products.Where(c =>
                    (c.Id.ToString().Contains(value)));
                    break;
                default:
                    break;
            }
            return products;
        }



        [HttpGet("~/api/orders/{id}/products")]
        public async Task<ActionResult<RequestResult<Products>>> GetOrderProducts(
            [FromRoute] int id,
            [FromQuery] List<FilterType> filters,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] SortType sort = SortType.IdAccending)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            IQueryable<Products> query = await GetQuery<Products>();
            query = await OrderQuery(query, sort);
            query = await FilterEntities(query, filters);
            query = query.Where(o => o.OrderId == id);
            var pageResult = await Paginate(query, page, pageSize);
            return Ok(pageResult);
        }

        [HttpPost("{id}/manufacture")]
        public async Task<IActionResult> ManufactureProduct([FromRoute] int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            await _workflowService.SyncTasksForProduct(product);

            return Ok("Product synced to manufacturing successfully.");
        }

        [HttpPost("{id}/pause")]
        public async Task<IActionResult> PauseProduct([FromRoute] int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound("Product not found.");

            await _workflowService.PauseProductionForProduct(product);

            return Ok("Production paused. Remaining tasks cancelled.");
        }

    }
}
