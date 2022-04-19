using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using RouteAdministration.ApiPerson.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RouteAdministration.ApiPerson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiPersonController : ControllerBase
    {
        private readonly ApiPersonService _personService;
        IWebHostEnvironment _appEnvironment;

        public ApiPersonController(ApiPersonService personService, IWebHostEnvironment env)
        {
            _personService = personService;
            _appEnvironment = env;
        }

        [HttpGet]
        public ActionResult<List<Person>> Get()
        {
            var people = _personService.Get();

            if (people == null)
                return BadRequest("Pessoa - A Api esta fora do ar. Tente novamente em instantes.");

            return people;
        }

        [HttpGet("{id}", Name = "GetPerson")]
        public ActionResult<Person> Get(string id)
        {
            var person = _personService.Get(id);

            if (person == null)
                return NotFound();

            return person;
        }

        [HttpPost]
        public ActionResult<Person> Create(Person person)
        {
            var personInsert = _personService.Create(person);

            if (personInsert == null)
                return BadRequest("Pessoa - A Api esta fora do ar. Tente novamente em instantes.");

            return CreatedAtRoute("GetPerson", new { id = person.Id }, person);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, Person personIn)
        {
            Person person = new();

            person = _personService.Get(id);

            if (person == null)
                return NotFound();

            _personService.Update(id, personIn);

            return CreatedAtRoute("GetPerson", new { id = personIn.Id }, personIn);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id, Person personIn)
        {
            Person person = new();

            person = _personService.Get(id);

            if (person == null)
                return NotFound();

            _personService.Remove(personIn.Id, personIn);

            return Ok();
        }
    }
}
