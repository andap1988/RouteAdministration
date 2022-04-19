using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RouteAdministration.ApiEquip.Model;
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
    }
}
