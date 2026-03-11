using Microsoft.AspNetCore.Mvc;
using Models.Attributes;
using Saad_Web_API.Data;

namespace Saad_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreasController : BasicController<PatternAreas>
    {
        public AreasController(ApplicationDbContext context) : base(context)
        {
        }
    }
}