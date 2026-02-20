using Microsoft.AspNetCore.Mvc;
using RESTBottleProject.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RESTBottleProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BottlesController : ControllerBase
    {
        private IBottlesRepository _repo; 

        public BottlesController(IBottlesRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<BottlesController>
        [HttpGet]
        public IEnumerable<Bottle> Get()
        {
            return _repo.GetAllBottles(); 
        }

        // GET api/<BottlesController>/5
        [HttpGet("{id}")]
        public Bottle? Get(int id)
        {
            return _repo.GetBottleById(id);
        }

        // POST api/<BottlesController>
        [HttpPost]
        public Bottle? Post([FromBody] Bottle newBottle)
        {
            return _repo.AddBottle(newBottle);
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
