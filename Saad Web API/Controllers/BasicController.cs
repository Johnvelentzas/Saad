using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Saad_Web_API.Data;

namespace Saad_Web_API.Controllers
{
    public class BasicController<T> : ControllerBase where T : class, IEntity
    {
        protected readonly ApplicationDbContext _context;
        public BasicController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET api/[controller]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T>>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }
        // GET api/[controller]/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<T>> GetById(
            [FromRoute] int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(entity);
        }

        // PUT api/[controller]/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            [FromRoute] int id,
            [FromBody] T entity)
        {
            if (id != entity.Id)
            {
                return BadRequest("The id provided doesnt match the entity id");
            }
            _context.Entry(entity).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Set<T>().Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // POST api/[controller]
        [HttpPost]
        public async Task<ActionResult<T>> Create(
            [FromBody] T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        // DELETE api/[controller]/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            [FromRoute] int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
