using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using RouteAdministration.ApiEquip.Service;
using System.Collections.Generic;

namespace RouteAdministration.ApiEquip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiEquipController : ControllerBase
    {
        private readonly ApiEquipService _equipService;

        public ApiEquipController(ApiEquipService equipService)
        {
            _equipService = equipService;
        }

        [HttpGet]
        public ActionResult<List<Equip>> Get()
        {
            var equip = _equipService.Get();

            if (equip == null)
                return BadRequest("Equipe - A Api esta fora do ar. Tente novamente em instantes.");

            return equip;
        }

        [HttpGet("{id}", Name = "GetEquip")]
        public ActionResult<Equip> Get(string id)
        {
            var equip = _equipService.Get(id);

            if (equip == null)
                return NotFound();

            return equip;
        }

        [HttpGet("city/{city}")]
        public ActionResult<List<Equip>> GetCity(string city)
        {
            var equips = _equipService.GetCity(city);

            if (equips == null)
                return NotFound();

            return equips;
        }

        [HttpGet("equip/{equipName}")]
        public ActionResult<Equip> GetEquipsByEquipName(string equipName)
        {
            var equip = _equipService.GetEquipsByEquipName(equipName);

            if (equip == null)
                return NotFound();

            return equip;
        }

        [HttpPost]
        public ActionResult<Equip> Create(Equip equip)
        {
            var equipInsertion = _equipService.Create(equip);

            if (equipInsertion == null)
                return BadRequest("Equipe - A Api esta fora do ar. Tente novamente em instantes.");

            return CreatedAtRoute("GetEquip", new { id = equip.Id }, equip);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, Equip equipIn)
        {
            Equip equip = new();

            equip = _equipService.Get(id);

            if (equip == null)
                return NotFound();

            _equipService.Update(id, equipIn);

            return CreatedAtRoute("GetEquip", new { id = equipIn.Id }, equipIn);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            Equip equip = new();

            equip = _equipService.Get(id);

            if (equip == null)
                return NotFound();

            _equipService.Remove(equip.Id, equip);

            return Ok();
        }
    }
}
