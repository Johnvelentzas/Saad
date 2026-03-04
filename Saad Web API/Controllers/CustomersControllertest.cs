using Microsoft.AspNetCore.Mvc;
using Models.Finances;

namespace Saad_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersControllertest : ControllerBase
    {
        // GET: api/<CustomersController>
        [HttpGet]
        public IEnumerable<Customers> Get()
        {
            return null;
            //TODO : Implement the logic to get all customers from the database and return them as a
        }

        // GET api/<CustomersController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<CustomersController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CustomersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CustomersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
