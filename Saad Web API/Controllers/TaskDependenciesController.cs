using Microsoft.AspNetCore.Mvc;
using Models.Attributes;
using Models.Production;
using Saad_Web_API.Data;

namespace Saad_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskDependenciesController : BasicController<TaskDependencies>
    {
        public TaskDependenciesController(ApplicationDbContext context) : base(context)
        {
        }
    }
}