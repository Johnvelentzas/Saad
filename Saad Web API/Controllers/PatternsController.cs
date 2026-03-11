using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Attributes;
using Saad_Web_API.Data;

namespace Saad_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatternsController : BasicController<Patterns>
    {
        public PatternsController(ApplicationDbContext context) : base(context)
        {
        }

        //GET api/patterns/{id}/areas
        [HttpGet("{id}/areas")]
        public async Task<ActionResult<IEnumerable<Patterns>>> GetAreasFromPatterns(
            [FromRoute] int id)
        {
            var pattern = await _context.Patterns.FindAsync(id);
            if (pattern == null)
            {
                return NotFound("Pattern doesn't exist");
            }
            var patterns = await _context.Patterns.Where(o => o.ModelId == id).ToListAsync();
            return Ok(patterns);
        }
    }
}