using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using RouteAdministration.Frontend.Service;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace RouteAdministration.Frontend.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            string user = "Anonymous";
            bool authenticate = false;

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                user = HttpContext.User.Identity.Name;
                authenticate = true;

                if (HttpContext.User.IsInRole("adm"))
                    ViewBag.Role = "adm";
                else
                    ViewBag.Role = "user";

            }
            else
            {
                user = "Não Logado";
                authenticate = false;
                ViewBag.Role = "";
            }

            ViewBag.User = user;
            ViewBag.Authenticate = authenticate;

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<User>> Login(User userLogin)
        {
            var userLoginSearch = await new ConnectToUserApi().GetUserByUsername(userLogin.Username);

            if (userLoginSearch != null)
            {
                if (userLogin.Password == userLoginSearch.Password)
                {
                    List<Claim> userClaims = new()
                    {
                        new Claim(ClaimTypes.Name, userLoginSearch.Username),
                        new Claim("Role", userLoginSearch.Role),
                        new Claim(ClaimTypes.Role, userLoginSearch.Role),
                    };

                    var myIdentity = new ClaimsIdentity(userClaims, "User");
                    var userPrincipal = new ClaimsPrincipal(new[] { myIdentity });

                    await HttpContext.SignInAsync(userPrincipal);

                    return RedirectToRoute(new { controller = "Upload", action = "Index" });
                }
            }

            ViewBag.Message = "Usuário ou senha incorretos.";            

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
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

            if (userLoginInsertion == null || userLoginInsertion.Error != "")
                return BadRequest("Usuário - Houve um erro na gravação do novo usuário. Favor tentar novamente");

            return RedirectToAction(nameof(Index));
        }
    }
}
