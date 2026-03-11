using Microsoft.AspNetCore.Mvc;
using Models.Attributes;
using Saad_Web_API.Data;

namespace Saad_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskDependeciesController : BasicController<PatternAreas>
    {
        public TaskDependeciesController(ApplicationDbContext context) : base(context)
        {
        }
    }
}