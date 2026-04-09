using Microsoft.AspNetCore.Mvc;
using Models;
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


        protected override async Task<IQueryable<Customers>> FilterEntities(IQueryable<Customers> customers, FilterType filter)
        {
            return filter switch
            {
                FilterType.Retail => customers.Where(c => c.Type == CustomerType.Retail),
                FilterType.Wholesale => customers.Where(c => c.Type == CustomerType.Wholesale),
                _ => customers,
            };
        }


        protected override async Task<IQueryable<Customers>> SearchEntities(IQueryable<Customers> customers, SearchType type, string value)
        {
            customers = await base.SearchEntities(customers, type, value);
            switch (type)
            {
                case SearchType.General:
                    customers = customers.Where(c =>
                    (c.Id.ToString().ToLower().Contains(value)) ||
                    (c.FirstName != null && c.FirstName.ToLower().Contains(value))||
                    (c.LastName.ToLower().Contains(value)) ||
                    (c.Email != null && c.Email.ToLower().ToLower().Contains(value.ToLower()))||
                    (c.TaxNumber != null && c.TaxNumber.ToLower().Contains(value))||
                    (c.Telephone != null && c.Telephone.ToLower().Contains(value)));
                    break;
                case SearchType.Name:
                    customers = customers.Where(c => (c.FirstName != null && c.FirstName.ToLower().Contains(value)) || c.LastName.ToLower().Contains(value));
                    break;
                case SearchType.Email:
                    customers = customers.Where(c => c.Email != null && c.Email.ToLower().Contains(value.ToLower()));
                    break;
                case SearchType.TaxNumber:
                    customers = customers.Where(c => c.TaxNumber != null && c.TaxNumber.ToLower().Contains(value));
                    break;
                case SearchType.PhoneNumber:
                    customers = customers.Where(c => c.Telephone != null && c.Telephone.ToLower().Contains(value));
                    break;
                default:
                    break;
            }
            return customers;
        }


        //GET api/customers/{id}/orders?{page}&{pagesize}&{sort}
        [HttpGet("{id}/orders")]
        public async Task<ActionResult<RequestResult<Orders>>> GetCustomerOrders(
            [FromRoute] int id,
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
            query = await OrderQuery(query);
            query = query.Where(o => o.CustomerId == id);
            var pageResult = await Paginate(query, page, pageSize);
            return Ok(pageResult);
        }
    }
}
