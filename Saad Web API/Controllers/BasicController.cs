using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

using Saad_Web_API.Data;



namespace Saad_Web_API.Controllers
{
    public class BasicController<T> : ControllerBase where T : class, IEntity
    {
        //This is the access to the database
        protected readonly ApplicationDbContext _context;
        public BasicController(ApplicationDbContext context)
        {
            _context = context;
        }

        internal async Task<IQueryable<T>> FilterEntities(IQueryable<T> entities, List<FilterType> filters)
        {
            if (filters == null || filters.Count == 0) { return entities; }
            foreach (var filter in filters)
            {
                entities = await FilterEntities(entities, filter);
            }
            return entities;
        }

        
        protected virtual async Task<IQueryable<T>> FilterEntities(IQueryable<T> entities, FilterType filter)
        {
            return entities;
        }

        /*
        internal async Task<IQueryable<T>> SearchEntities(IQueryable<T> entities, List<SearchTerm> terms)
        {
            if (terms == null || terms.Count == 0) { return entities; }
            foreach (var term in terms)
            {
                entities = await SearchEntities(entities, term);
            }
            return entities;
        }
        */

        protected virtual async Task<IQueryable<T>> SearchEntities(IQueryable<T> entities, SearchType type, string value)
        {
            switch (type)
            {
                case SearchType.Id:
                    entities = entities.Where(e => e.Id.ToString().Contains(value));
                    break;
            }

            return entities;
        }


        internal async Task<IOrderedQueryable<A>> OrderQuery<A>(
            IQueryable<A> query,
            SortType sort = SortType.IdDecending) 
            where A : class, IEntity
        {
            switch (sort)
            {
                case SortType.IdDecending:
                    return query.OrderByDescending(r => r.Id);
                default:
                    return query.OrderBy(r => r.Id);
            }
        }

        internal async Task<IQueryable<A>> GetQuery<A>() 
            where A : class, IEntity
        {
            return _context.Set<A>().AsQueryable();
        }

        internal async Task<RequestResult<A>> Paginate<A>(
            IQueryable<A> query,
            int page = 1,
            int pageSize = 100) 
            where A : class, IEntity
        {
            query = query.Skip((page - 1) * pageSize).Take(pageSize);
            int totalCount = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            int currentPage = page;
            RequestResult<A> result = new RequestResult<A>(await query.ToListAsync(), totalCount, totalPages, currentPage);
            return result;
        }

        //--------------------------------------------------------------
        // The following are the basic CRUD operations for every entity
        //--------------------------------------------------------------

        // GET api/[controller]?{page}&{pagesize}&{sort}
        [HttpGet]
        public async Task<ActionResult<RequestResult<T>>> GetPage(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] SortType sort = SortType.IdDecending)
        {
            IQueryable<T> query = await GetQuery<T>();
            query = await OrderQuery(query, sort);
            var pageResult = await Paginate(query, page, pageSize);
            return Ok(pageResult);
        }

        // GET api/[controller]/search?{filters}&{searchtype}&{searchvalue}&{page}&{pagesize}&{sort}
        [HttpGet("search")]
        public async Task<ActionResult<RequestResult<T>>> GetSearchPage(
            [FromQuery] List<FilterType> filters,
            [FromQuery] SearchType searchType,
            [FromQuery] string searchValue,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] SortType sort = SortType.IdDecending)
        {
            IQueryable<T> query = await GetQuery<T>();
            query = await OrderQuery(query, sort);
            query = await FilterEntities(query, filters);
            query = await SearchEntities(query, searchType, searchValue);
            var pageResult = await Paginate(query, page, pageSize);
            return Ok(pageResult);
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
