using Microsoft.AspNetCore.Mvc;
using Models.Production;
using Saad_Web_API.Data;
using System.Diagnostics;

namespace Saad_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessesController : BasicController<Processes>
    {
        public ProcessesController(ApplicationDbContext context) : base(context)
        {
        }
    }
}
