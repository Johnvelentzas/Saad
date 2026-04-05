using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Finances;
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

        protected override async Task<IQueryable<Orders>> FilterEntities(IQueryable<Orders> orders, FilterType filter)
        {
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


        //GET api/orders/{id}/products?{page}&{pagesize}&{sort}
        [HttpGet("{id}/products")]
        public async Task<ActionResult<RequestResult<Products>>> GetOrderProducts(
            [FromRoute] int id,
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
            query = await OrderQuery(query);
            query = query.Where(o => o.OrderId == id);
            var pageResult = await Paginate(query, page, pageSize);
            return Ok(pageResult);
        }
    }
}