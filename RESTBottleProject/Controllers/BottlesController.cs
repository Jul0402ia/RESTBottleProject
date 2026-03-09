using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using RESTBottleProject.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RESTBottleProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BottlesController : ControllerBase
    {
        private IBottlesRepository _repo; 

        public BottlesController(IBottlesRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<BottlesController>
        [HttpGet]
        public IEnumerable<Bottle> Get([FromQuery] string substring)
        {
            return _repo.GetAllBottles(substring); 
        }

        // GET api/<BottlesController>/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public ActionResult<Bottle> Get(int id)
        {
            var bottle = _repo.GetBottleById(id);
            if (bottle == null) return NotFound("No such bottle, id: " + id);
            return Ok(bottle);
        }

        // POST api/<BottlesController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Bottle> Post([FromBody] Bottle newBottle)
        {
            try
            { _repo.AddBottle(newBottle);
                return Created($"api/items/{newBottle.Id}", newBottle);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<BottlesController>/5
        [HttpPut("{id}")]
        public Bottle? Put(int id, [FromBody] Bottle value)
        {
            return _repo.UpdateBottle(id, value);
        }

        // DELETE api/<BottlesController>/5
        [HttpDelete("{id}")]
        public Bottle? Delete(int id)
        {
            return _repo.RemoveBottle(id);
        }
    }
}
