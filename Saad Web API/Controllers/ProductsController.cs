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

        //GET api/Products/search?orderId=1&categoryId=2
        //TODO : Implement the logic to search a prodcut by varius parameters and return the matching products as a response.
    }
}
