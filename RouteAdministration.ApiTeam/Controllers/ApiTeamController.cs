using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RouteAdministration.ApiTeam.Model;
using RouteAdministration.ApiTeam.Service;
using System.Collections.Generic;

namespace RouteAdministration.ApiTeam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiTeamController : ControllerBase
    {
        /*private readonly ApiTeamService _teamService;
        IWebHostEnvironment _appEnvironment;

        public ApiTeamController(ApiTeamService teamService, IWebHostEnvironment env)
        {
            _teamService = teamService;
            _appEnvironment = env;
        }

        [HttpGet]
        public ActionResult<List<Team>> Get()
        {
            var team = _teamService.Get();

            if (team == null)
                return BadRequest("Equipe - A Api esta fora do ar. Tente novamente em instantes.");

            return team;
        }

        [HttpGet("{id}", Name = "GetTeam")]
        public ActionResult<Team> Get(string id)
        {
            var team = _teamService.Get(id);

            if (team == null)
                return NotFound();

            return team;
        }

        [HttpPost]
        public ActionResult<Team> Create(Team team)
        {
            var teamInsertion = _teamService.Create(team);

            if (teamInsertion == null)
                return BadRequest("Equipe - A Api esta fora do ar. Tente novamente em instantes.");

            return CreatedAtRoute("GetTeam", new { id = team.Id }, team);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, Team teamIn)
        {
            Team team = new();

            team = _teamService.Get(id);

            if (team == null)
                return NotFound();

            _teamService.Update(id, teamIn);

            return CreatedAtRoute("GetTeam", new { id = teamIn.Id }, teamIn);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id, Team teamIn)
        {
            Team team = new();

            team = _teamService.Get(id);

            if (team == null)
                return NotFound();

            _teamService.Remove(teamIn.Id, teamIn);

            return Ok();
        }*/
    }
}
