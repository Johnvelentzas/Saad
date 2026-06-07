using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Attributes;
using Saad_Web_API.Data;

namespace Saad_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : BasicController<Brands>
    {
        public BrandsController(ApplicationDbContext context) : base(context)
        {
        }


    }
}
