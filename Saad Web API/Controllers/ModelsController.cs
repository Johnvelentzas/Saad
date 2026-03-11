using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Attributes;
using Saad_Web_API.Data;

namespace Saad_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelsController : BasicController<Models.Attributes.Models>
    {
        public ModelsController(ApplicationDbContext context) : base(context)
        {
        }

        //GET api/models/{id}/patterns
        [HttpGet("{id}/patterns")]
        public async Task<ActionResult<IEnumerable<Patterns>>> GetPatternsFromModel(
            [FromRoute] int id)
        {
            var model = await _context.Models.FindAsync(id);
            if (model == null)
            {
                return NotFound("Model doesn't exist");
            }
            var patterns = await _context.Patterns.Where(o => o.ModelId == id).ToListAsync();
            return Ok(patterns);
        }
    }
}