using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using RouteAdministration.ApiCity.Service;
using System.Collections.Generic;

namespace RouteAdministration.ApiCity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiCityController : ControllerBase
    {
        private readonly ApiCityService _cityService;

        public ApiCityController(ApiCityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet]
        public ActionResult<List<City>> Get()
        {
            var city = _cityService.Get();

            if (city == null)
                return BadRequest("Cidade - A Api esta fora do ar. Tente novamente em instantes.");

            return city;
        }

        [HttpGet("{id}", Name = "GetCity")]
        public ActionResult<City> Get(string id)
        {
            var city = _cityService.Get(id);

            if (city == null)
                return NotFound();

            return city;
        }

        [HttpPost]
        public ActionResult<City> Create(City city)
        {
            var cityInsertion = _cityService.Create(city);

            if (cityInsertion == null)
                return BadRequest("Cidade - A Api esta fora do ar. Tente novamente em instantes.");

            return CreatedAtRoute("GetCity", new { id = city.Id }, city);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, City cityIn)
        {
            City city = new();

            city = _cityService.Get(id);

            if (city == null)
                return NotFound();

            _cityService.Update(id, cityIn);

            return CreatedAtRoute("GetCity", new { id = cityIn.Id }, cityIn);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id, City cityIn)
        {
            City equip = new();

            equip = _cityService.Get(id);

            if (equip == null)
                return NotFound();

            _cityService.Remove(cityIn.Id, cityIn);

            return Ok();
        }
    }
}
