using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Attributes;
using Models.Production;
using Saad_Web_API.Data;

namespace Saad_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProcessesController : BasicController<UserProcesses>
    {
        public UserProcessesController(ApplicationDbContext context) : base(context)
        {
        }
    }
}