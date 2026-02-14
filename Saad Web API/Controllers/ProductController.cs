using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Saad_Web_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {


        [HttpGet]
        public ActionResult Get()
        {
            return Ok("This is the product controller");
            //TODO : Implement the logic to get all products from the database and return them as a response.
            
        }

        //TODO : Implement the logic to get a product by id from the database and return it as a response.
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            return Ok();
        }
        //TODO : Implement the logic to create a new product in the database and return the created product as a response.
        //TODO : Implement the logic to update a product in the database and return the updated product as a response.
        //TODO : Implement the logic to delete a product from the database and return a response indicating the success of the operation.
        //TODO : Implement the logic to search a prodcut by varius parameters and return the matching products as a response.
        }
}
