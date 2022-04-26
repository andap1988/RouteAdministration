using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using RouteAdministration.Frontend.Models;
using RouteAdministration.Frontend.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RouteAdministration.Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        IWebHostEnvironment _appEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _appEnvironment = env;
        }

        public async Task<IActionResult> Index()
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

            //var result = await new ConnectToRadarApi().GetLocation();

            List<User> users = new();

            users = await new ConnectToUserApi().GetUsers();
            var userTemp = await new ConnectToUserApi().GetUserByUsername("temp");

            if (userTemp == null && users.Count < 1)
            {
                userTemp = await new ConnectToUserApi().CreateNewUser(new User() { Username = "temp", Password = "temp" });

                ViewBag.User = user;
                ViewBag.Authenticate = authenticate;

                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            ViewBag.User = user;
            ViewBag.Authenticate = authenticate;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
