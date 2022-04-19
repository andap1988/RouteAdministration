using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using RouteAdministration.Frontend.Service;
using System.Threading.Tasks;

namespace RouteAdministration.Frontend.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            //var users = await new ConnectToUserApi().GetUsers();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<User>> Login(User userLogin)
        {
            var userLoginSearch = await new ConnectToUserApi().GetUserByUsername(userLogin.Username);

            if (userLoginSearch != null)
            {
                userLoginSearch.Password = "";

                HttpContext.Session.SetString("username", userLoginSearch.Username);
            }

            return RedirectToRoute(new { controller = "Home", action = "Index" });
        }

        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<User>> Create(User userLogin)
        {
            var userLoginInsertion = await new ConnectToUserApi().CreateNewUser(userLogin);

            if (userLoginInsertion != null)
                return BadRequest("Usuário - Houve um erro na gravação do novo usuário. Favor tentar novamente");

            return RedirectToAction(nameof(Index));
        }
    }
}
