using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Production;
using Saad_Web_API.Data;
using Saad_Web_API.Helpers;

namespace Saad_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : BasicController<Orders>
    {
        private readonly IProductionWorkflowService _workflowService;

        public OrdersController(ApplicationDbContext context, IProductionWorkflowService workflowService) : base(context)
        {
            _workflowService = workflowService;
        }

        protected override async Task<IQueryable<Orders>> FilterEntities(IQueryable<Orders> orders, FilterType filter)
        {
            orders = await base.FilterEntities(orders, filter);
            return filter switch
            {
                FilterType.Complete => orders.Where(c => c.IsCompleted),
                FilterType.Incomplete => orders.Where(c => !c.IsCompleted),
                FilterType.InStore => orders.Where(c => c.SaleChannel == SaleChannel.InStore),
                FilterType.Phone => orders.Where(c => c.SaleChannel == SaleChannel.Phone),
                FilterType.Online => orders.Where(c => c.SaleChannel == SaleChannel.Online),
                FilterType.SocialMedia => orders.Where(c => c.SaleChannel == SaleChannel.SocialMedia),
                FilterType.Email => orders.Where(c => c.SaleChannel == SaleChannel.Email),
                _ => orders,
            };
        }


        protected override async Task<IQueryable<Orders>> SearchEntities(IQueryable<Orders> orders, SearchType type, string value)
        {
            orders = await base.SearchEntities(orders, type, value);
            switch (type)
            {
                case SearchType.General:
                    orders = orders.Where(c =>
                    (c.Id.ToString().Contains(value)));
                    break;
                default:
                    break;
            }
            return orders;
        }

        [HttpGet("~/api/customers/{id}/orders")]
        public async Task<ActionResult<RequestResult<Orders>>> GetCustomerOrders(
            [FromRoute] int id,
            [FromQuery] List<FilterType> filters,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] SortType sort = SortType.IdDecending)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            IQueryable<Orders> query = await GetQuery<Orders>();
            query = await OrderQuery(query, sort);
            query = await FilterEntities(query, filters);
            query = query.Where(o => o.CustomerId == id);
            var pageResult = await Paginate(query, page, pageSize);
            return Ok(pageResult);
        }

        [HttpPost("{id}/manufacture")]
        public async Task<IActionResult> ManufactureOrder([FromRoute] int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            // 1. Get the total count of products just so we know the overall state
            var totalProductsCount = await _context.Products.CountAsync(p => p.OrderId == id);

            if (totalProductsCount == 0)
            {
                return BadRequest("This order has no products to manufacture.");
            }

            // 2. THE FIX: Fetch ONLY the finalized (non-draft) products
            var readyProducts = await _context.Products
                .Where(p => p.OrderId == id && p.IsDraft == false)
                .ToListAsync();

            if (!readyProducts.Any())
            {
                // 3. Prevent the action entirely if they click Manufacture on an order full of drafts
                return BadRequest("All products in this order are currently drafts. Please finalize a product first.");
            }

            // 4. Sync tasks for only the finalized products
            foreach (var product in readyProducts)
            {
                await _workflowService.SyncTasksForProduct(product);
            }

            // 5. Build a smart response message for the MAUI UI
            int skippedDrafts = totalProductsCount - readyProducts.Count;

            if (skippedDrafts > 0)
            {
                return Ok($"Sent {readyProducts.Count} product(s) to manufacturing. Ignored {skippedDrafts} draft(s).");
            }

            return Ok($"Successfully synced all {readyProducts.Count} product(s) to manufacturing.");
        }

        [HttpPost("{id}/pause")]
        public async Task<IActionResult> PauseOrder([FromRoute] int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound("Order not found.");

            // Find all products in this order that are currently in production
            var activeProducts = await _context.Products
                .Where(p => p.OrderId == id && p.HasStartedManufacturing == true)
                .ToListAsync();

            if (!activeProducts.Any()) return BadRequest("No active products to pause.");

            foreach (var product in activeProducts)
            {
                await _workflowService.PauseProductionForProduct(product);
            }

            return Ok($"Successfully paused production for {activeProducts.Count} product(s).");
        }
    }
}