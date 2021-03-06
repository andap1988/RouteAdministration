using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using RouteAdministration.ApiUser.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RouteAdministration.ApiUser.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiUserController : ControllerBase
    {
        private readonly ApiUserService _userService;
        IWebHostEnvironment _appEnvironment;

        public ApiUserController(ApiUserService userService, IWebHostEnvironment env)
        {
            _userService = userService;
            _appEnvironment = env;
        }

        [HttpGet]
        public ActionResult<List<User>> Get()
        {
            var user = _userService.Get();

            if (user == null)
                return BadRequest("User - A Api esta fora do ar. Tente novamente em instantes.");

            return user;
        }

        [HttpGet("{id}", Name = "GetUser")]
        public ActionResult<User> Get(string id)
        {
            var user = _userService.Get(id);

            if (user == null)
                return NotFound();

            return user;
        }

        [HttpGet("login/{username}")]
        public ActionResult<User> GetUserByUsername(string username)
        {
            var user = _userService.GetUserByUsername(username);

            if (user == null)
                return NotFound();

            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create(User user)
        {
            var userInsertion = await _userService.Create(user);

            if (userInsertion.Error == "yesUser")
                return BadRequest("User - O usuário já está cadastrado no sistema.");

            if (userInsertion == null)
                return BadRequest("User - A Api esta fora do ar. Tente novamente em instantes.");

            return CreatedAtRoute("GetUser", new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, User userIn)
        {
            User user = new();

            user = _userService.Get(id);

            if (user == null)
                return NotFound();

            _userService.Update(id, userIn);

            return CreatedAtRoute("GetUser", new { id = userIn.Id }, userIn);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            User user = new();

            user = _userService.Get(id);

            if (user == null)
                return NotFound();

            _userService.Remove(user.Id, user);

            return Ok();
        }
    }
}
