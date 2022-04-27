using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using RouteAdministration.Frontend.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RouteAdministration.Frontend.Controllers
{
    [Authorize]
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

            if (user == "temp")
                return View();

            var equips = await new ConnectToEquipApi().GetEquips();

            if (equips == null)
            {
                TempData["error"] = "Equipe - A API está fora do ar. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            equips.Sort((equipOne, equipTwo) => equipOne.Name.CompareTo(equipTwo.Name));

            return View(equips);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.User = HttpContext.User.Identity.Name;

            if (HttpContext.User.IsInRole("adm"))
                ViewBag.Role = "adm";
            else
                ViewBag.Role = "user";

            var people = await new ConnectToPersonApi().GetPeople();

            if (people == null || people[0].Error != "")
            {
                TempData["error"] = "Equipe - A API está fora do ar. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            var cities = await new ConnectToCityApi().GetCities();

            if (cities == null || cities[0].Error != "")
            {
                TempData["error"] = "Equipe - A API está fora do ar. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            List<Person> peopleWithoutTem = people.FindAll(person => person.TeamName == "");

            peopleWithoutTem.Sort((personOne, personTwo) => personOne.Name.CompareTo(personTwo.Name));

            ViewBag.People = peopleWithoutTem;
            ViewBag.Cities = cities;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Equip>> Create(Equip equip, List<string> selectedPerson)
        {
            if (string.IsNullOrEmpty(equip.Name) || string.IsNullOrWhiteSpace(equip.Name))
            {
                TempData["error"] = "O nome da equipe é obrigatório.";

                return RedirectToAction(nameof(Index));
            }
            else if (selectedPerson.Count < 1)
            {
                TempData["error"] = "Para cadastrar uma equipe, é necessário colocar pelo menos uma pessoa.";

                return RedirectToAction(nameof(Index));
            }

            List<Person> peopleChoices = new();

            foreach (string selPerson in selectedPerson)
            {
                Person person = await new ConnectToPersonApi().GetPersonByName(selPerson);

                if (person == null || person.Error != "")
                {
                    TempData["error"] = "Pessoa - A API está fora do ar. Favor tentar novamente.";

                    return RedirectToAction(nameof(Index));
                }

                person.TeamName = equip.Name;

                peopleChoices.Add(person);
            }

            equip.People = peopleChoices;

            var equipInsertion = await new ConnectToEquipApi().CreateNewEquip(equip);

            if (equipInsertion == null || equipInsertion.Error != "")
            {
                TempData["error"] = "Equipe - Houve um erro na gravação da nova equipe. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            foreach (Person person in peopleChoices)
            {
                var personInsertion = await new ConnectToPersonApi().EditPerson(person);

                if (equipInsertion == null || equipInsertion.Error != "")
                {
                    TempData["error"] = "Pessoa - A API está fora do ar. Favor tentar novamente.";

                    return RedirectToAction(nameof(Index));
                }
            }

            TempData["success"] = "Equipe criada com sucesso!";

            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> Edit(string id)
        {
            ViewBag.User = HttpContext.User.Identity.Name;

            if (HttpContext.User.IsInRole("adm"))
                ViewBag.Role = "adm";
            else
                ViewBag.Role = "user";

            if (id == null)
            {
                TempData["error"] = "Equipe - Houve um erro na página. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            var equip = await new ConnectToEquipApi().GetEquipById(id);

            if (equip == null || equip.Error != "")
            {
                TempData["error"] = "Equipe - A API está fora do ar. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            var cities = await new ConnectToCityApi().GetCities();

            if (cities == null || equip.Error != "")
            {
                TempData["error"] = "Equipe - A API está fora do ar. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            var people = await new ConnectToPersonApi().GetPeople();

            if (people == null || people[0].Error != "")
            {
                TempData["error"] = "Equipe - A API está fora do ar. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            List<Person> listPersonWithoutEquip = new();

            people.ForEach(person =>
            {
                if (person.TeamName == "")
                    listPersonWithoutEquip.Add(person);
            });

            ViewBag.PeopleWithoutEquip = listPersonWithoutEquip;
            ViewBag.Cities = cities;

            return View(equip);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Equip equip, List<string> selectedPeople, List<string> selectedPeopleWithoutEquip)
        {
            if (string.IsNullOrEmpty(equip.Name) || string.IsNullOrWhiteSpace(equip.Name))
            {
                TempData["error"] = "O nome da equipe não pode ser vazio.";

                return RedirectToAction(nameof(Index));
            }

            var equipSearch = await new ConnectToEquipApi().GetEquipById(equip.Id);

            if (equipSearch == null)
            {
                TempData["error"] = "Equipe - A API está fora do ar. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            var oldPeopleInEquip = equipSearch.People;

            equipSearch.Name = equip.Name;
            equipSearch.City = equip.City;
            equipSearch.City = equip.City;

            if (equipSearch.People.Count == selectedPeople.Count && selectedPeopleWithoutEquip.Count < 1)
            {
                var equipInsertion = await new ConnectToEquipApi().EditEquip(equipSearch);

                if (equipInsertion == null)
                {
                    TempData["error"] = "Equipe - A API está fora do ar. Favor tentar novamente.";

                    return RedirectToAction(nameof(Edit));
                }

                foreach (Person person in equipInsertion.People)
                {
                    person.TeamName = equipInsertion.Name;

                    var personInsertion = await new ConnectToPersonApi().EditPerson(person);

                    if (personInsertion == null)
                    {
                        TempData["error"] = "Pessoa - A API está fora do ar. Favor tentar novamente.";

                        return RedirectToAction(nameof(Index));
                    }
                }

                TempData["success"] = "Equipe editada com sucesso!";

                return RedirectToAction(nameof(Index));
            }

            List<Person> newPeople = new();

            if (selectedPeople.Count > 0)
            {
                foreach (var person in selectedPeople)
                {
                    Person personSearch = await new ConnectToPersonApi().GetPersonByName(person);
                    personSearch.TeamName = equipSearch.Name;
                    newPeople.Add(personSearch);
                };
            }

            if (selectedPeopleWithoutEquip.Count > 0)
            {
                foreach (var person in selectedPeopleWithoutEquip)
                {
                    Person personSearch = await new ConnectToPersonApi().GetPersonByName(person);
                    personSearch.TeamName = equipSearch.Name;
                    newPeople.Add(personSearch);
                }
            }

            equipSearch.People = newPeople;

            var equipInsertionWithNewPeople = await new ConnectToEquipApi().EditEquip(equipSearch);

            if (equipInsertionWithNewPeople == null)
            {
                TempData["error"] = "Equipe - A API está fora do ar. Favor tentar novamente.";

                return RedirectToAction(nameof(Edit));
            }

            foreach (Person person in equipInsertionWithNewPeople.People)
            {
                var personInsertion = await new ConnectToPersonApi().EditPerson(person);

                if (personInsertion == null)
                {
                    TempData["error"] = "Pessoa - A API está fora do ar. Favor tentar novamente.";

                    return RedirectToAction(nameof(Index));
                }
            }

            if (oldPeopleInEquip.Count > newPeople.Count)
            {
                foreach (Person oldPerson in oldPeopleInEquip)
                {
                    var person = newPeople.Find(person => person.Name == oldPerson.Name);
                    
                    if (person == null)
                    {
                        oldPerson.TeamName = "";

                        var editPerson = await new ConnectToPersonApi().EditPerson(oldPerson);

                        if (editPerson == null)
                        {
                            TempData["error"] = "Pessoa - A API está fora do ar. Favor tentar novamente.";

                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
            }

            TempData["success"] = "Equipe editada com sucesso!";

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
                TempData["error"] = "Equipe - Houve um erro na página. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            var equip = await new ConnectToEquipApi().GetEquipById(id);

            if (equip == null || equip.Error != "")
            {
                TempData["error"] = "Equipe - A API está fora do ar. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            return View(equip);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, Equip equipRemove)
        {
            var equip = await new ConnectToEquipApi().GetEquipById(id);

            if (equip == null || equip.Error != "")
            {
                TempData["error"] = "Equipe - A API está fora do ar. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            var equipRemoved = await new ConnectToEquipApi().RemoveEquip(id);

            if (equipRemoved.Error != "ok")
            {
                TempData["error"] = "Houve um erro na exclusão da equipe. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            foreach (Person person in equip.People)
            {
                person.TeamName = "";

                var personInsertion = await new ConnectToPersonApi().EditPerson(person);

                if (personInsertion == null || personInsertion.Error != "")
                {
                    TempData["error"] = "Pessoa - A API está fora do ar. Favor tentar novamente.";

                    return RedirectToAction(nameof(Index));
                }
            }

            TempData["success"] = "Equipe removida com sucesso!";

            return RedirectToAction(nameof(Index));
        }
    }
}
