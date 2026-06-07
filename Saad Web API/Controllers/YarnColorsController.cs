using Microsoft.AspNetCore.Mvc;
using Models.Attributes;
using Saad_Web_API.Data;

namespace Saad_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YarnColorsController : BasicController<YarnColors>
    {
        public YarnColorsController(ApplicationDbContext context) : base(context)
        {
        }
    }
}