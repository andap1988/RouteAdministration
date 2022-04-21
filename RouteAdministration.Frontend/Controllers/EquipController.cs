using Microsoft.AspNetCore.Mvc;
using Models;
using RouteAdministration.Frontend.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RouteAdministration.Frontend.Controllers
{
    public class EquipController : Controller
    {
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

            var equips = await new ConnectToEquipApi().GetEquips();

            return View(equips);
        }

        public async Task<IActionResult> Create()
        {
            var people = await new ConnectToPersonApi().GetPeople();
            var cities = await new ConnectToCityApi().GetCities();

            List<Person> peopleWithoutTem = people.FindAll(person => person.TeamName == "");

            ViewBag.People = peopleWithoutTem;
            ViewBag.Cities = cities;
    
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Equip>> Create(Equip equip, List<string> selectedPerson)
        {
            List<Person> peopleChoices = new();

            foreach(string selPerson in selectedPerson)
            {
                Person person = await new ConnectToPersonApi().GetPersonByName(selPerson);

                person.TeamName = equip.Name;

                peopleChoices.Add(person);
            }

            equip.People = peopleChoices;

            var equipInsertion = await new ConnectToEquipApi().CreateNewEquip(equip);

            if (equipInsertion == null)
                return BadRequest("Equipe - Houve um erro na gravação da nova equipe. Favor tentar novamente");

            foreach(Person person in peopleChoices)
            {
                await new ConnectToPersonApi().EditPerson(person);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equip = await new ConnectToEquipApi().GetEquipById(id);

            if (equip == null)
            {
                return NotFound();
            }

            return View(equip);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, Equip equipRemove)
        {
            var equip = await new ConnectToEquipApi().GetEquipById(id);
            var equipRemoved = await new ConnectToEquipApi().RemoveEquip(id);

            if (equipRemoved.Error != "ok")
                return BadRequest("Equipe - Houve um erro na exclusão da equipe. Favor tentar novamente");

            foreach(Person person in equip.People)
            {
                person.TeamName = "";

                await new ConnectToPersonApi().EditPerson(person);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
