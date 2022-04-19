using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RouteAdministration.Frontend.Models;
using RouteAdministration.Frontend.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RouteAdministration.Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            //var a = await new ConnectToPersonApi().GetPeople();
            //var a = await new ConnectToCityApi().GetCities();
            //var a = await new ConnectToEquipApi().GetEquips();
            //var a = await new ConnectToUserApi().GetUsers();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
