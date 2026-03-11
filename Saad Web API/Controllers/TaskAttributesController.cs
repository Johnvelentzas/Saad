using Microsoft.AspNetCore.Mvc;
using Models.Attributes;
using Saad_Web_API.Data;

namespace Saad_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskAttributesController : BasicController<TaskAtributes>
    {
        public TaskAttributesController(ApplicationDbContext context) : base(context)
        {
        }
    }
}