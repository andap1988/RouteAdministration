using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Models;
using RouteAdministration.Frontend.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RouteAdministration.Frontend.Controllers
{
    [Authorize]
    public class CityController : Controller
    {
        IWebHostEnvironment _appEnvironment;

        public CityController(IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
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

            ViewBag.User = user;
            ViewBag.Authenticate = authenticate;

            var cities = await new ConnectToCityApi().GetCities();

            if (cities == null)
            {
                TempData["error"] = "Cidade - A API está fora do ar. Favor tentar novamente";

                return RedirectToAction(nameof(Index));
            }

            cities.Sort((cityOne, cityTwo) => cityOne.Name.CompareTo(cityTwo.Name));

            return View(cities);
        }

        public IActionResult Create()
        {
            ViewBag.User = HttpContext.User.Identity.Name;

            if (HttpContext.User.IsInRole("adm"))
                ViewBag.Role = "adm";
            else
                ViewBag.Role = "user";

            List<City> cities = ReadFiles.ReadTXTCities(_appEnvironment.WebRootPath);

            ViewBag.Cities = cities;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<City>> Create(City city)
        {
            var cityInsertion = await new ConnectToCityApi().CreateNewCity(city);

            if (cityInsertion == null || cityInsertion.Error != "")
            {
                TempData["error"] = "Cidade - Houve um erro na gravação da nova cidade. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            TempData["success"] = "Cidade criada com sucesso!";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            ViewBag.User = HttpContext.User.Identity.Name;

            if (HttpContext.User.IsInRole("adm"))
                ViewBag.Role = "adm";
            else
                ViewBag.Role = "user";

            if (id == null)
            {
                TempData["error"] = "Cidade - Houve um erro na página. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            var city = await new ConnectToCityApi().GetCityById(id);

            if (city == null || city.Error != "")
            {
                TempData["error"] = "Cidade - A API está fora do ar. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }            

            return View(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, City city)
        {
            var cityRemove = await new ConnectToCityApi().RemoveCity(id);

            if (cityRemove.Error != "ok")
            {
                TempData["error"] = "Cidade - Houve um erro na exclusão da cidade. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            TempData["success"] = "Cidade removida com sucesso!";

            return RedirectToAction(nameof(Index));
        }
    }
}
