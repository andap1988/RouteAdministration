using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using RouteAdministration.Frontend.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RouteAdministration.Frontend.Controllers
{
    [Authorize]
    public class PersonController : Controller
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

            var people = await new ConnectToPersonApi().GetPeople();

            if (people == null || people[0].Error != "")
            {
                TempData["error"] = "Pessoa - A API está fora do ar. Favor tentar novamente";

                return RedirectToAction(nameof(Index));
            }

            return View(people);
        }

        public IActionResult Create()
        {
            ViewBag.User = HttpContext.User.Identity.Name;

            if (HttpContext.User.IsInRole("adm"))
                ViewBag.Role = "adm";
            else
                ViewBag.Role = "user";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Person>> Create(Person person)
        {
            if (string.IsNullOrEmpty(person.Name))
            {
                TempData["error"] = "O nome da pessoa é obrigatório";

                return RedirectToAction(nameof(Index));
            }

            var personInsertion = await new ConnectToPersonApi().CreateNewPerson(person);

            if (personInsertion == null || personInsertion.Error != null)
            {
                TempData["error"] = "Pessoa - Houve um erro na gravação da nova pessoa. Favor tentar novamente";

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            ViewBag.User = HttpContext.User.Identity.Name;

            if (HttpContext.User.IsInRole("adm"))
                ViewBag.Role = "adm";
            else
                ViewBag.Role = "user";

            if (id == null)
            {
                TempData["error"] = "Pessoa - Houve um erro na página. Favor tentar novamente";

                return RedirectToAction(nameof(Index));
            }

            var person = await new ConnectToPersonApi().GetPersonById(id);

            if (person == null || person.Error != "")
            {
                TempData["error"] = "Pessoa - A API está fora do ar. Favor tentar novamente";

                return RedirectToAction(nameof(Index));
            }

            return View(person);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Person person)
        {
            if (string.IsNullOrEmpty(person.Name))
            {
                TempData["error"] = "O nome da pessoa é obrigatório";

                return RedirectToAction(nameof(Index));
            }

            var personComplete = await new ConnectToPersonApi().GetPersonById(id);

            if (personComplete == null || personComplete.Error != "")
            {
                TempData["error"] = "Houve um erro na edição do novo nome da pessoa. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            if (!string.IsNullOrEmpty(personComplete.TeamName))
            {
                var equip = await new ConnectToEquipApi().GetEquipByEquipName(personComplete.TeamName);

                if (equip == null || equip.Error != "")
                {
                    TempData["error"] = "Pessoa - Houve um erro na edição da equipe com o novo nome da pessoa. Favor tentar novamente.";

                    return RedirectToAction(nameof(Index));
                }

                List<Person> peopleEquip = equip.People;
                List<Person> newPeopleTeam = new();

                peopleEquip.ForEach(personEquip =>
                {
                    if (personEquip.Name != personComplete.Name)
                        newPeopleTeam.Add(personEquip);
                });

                newPeopleTeam.Add(new Person() { Id = personComplete.Id, Name = person.Name, TeamName = personComplete.TeamName });

                equip.People = newPeopleTeam;

                var equipInsertion = await new ConnectToEquipApi().EditEquip(equip);

                if (equipInsertion == null || equipInsertion.Error != "")
                {
                    TempData["error"] = "Pessoa - Houve um erro na edição da equipe com o novo nome da pessoa. Favor tentar novamente.";

                    return RedirectToAction(nameof(Index));
                }
            }

            personComplete.Name = person.Name;

            var personEdit = await new ConnectToPersonApi().EditPerson(personComplete);

            if (personEdit == null || personEdit.Error != "")
            {
                TempData["error"] = "Pessoa - Houve um erro na edição da pessoa. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

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
                TempData["error"] = "Pessoa - Houve um erro na página. Favor tentar novamente";

                return RedirectToAction(nameof(Index));
            }

            var person = await new ConnectToPersonApi().GetPersonById(id);

            if (person == null || person.Error != "")
            {
                TempData["error"] = "Pessoa - A API está fora do ar. Favor tentar novamente";

                return RedirectToAction(nameof(Index));
            }

            return View(person);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, Person person)
        {
            var personComplete = await new ConnectToPersonApi().GetPersonById(id);

            if (personComplete == null || personComplete.Error != "")
            {
                TempData["error"] = "Pessoa - Houve um erro na exclusão da pessoa. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            if (!string.IsNullOrEmpty(personComplete.TeamName))
            {
                var equip = await new ConnectToEquipApi().GetEquipByEquipName(personComplete.TeamName);

                if (equip == null || equip.Error != "")
                {
                    TempData["error"] = "Pessoa - Houve um erro na edição da equipe com o nome da pessoa. Favor tentar novamente.";

                    return RedirectToAction(nameof(Index));
                }

                List<Person> peopleEquip = equip.People;
                List<Person> newPeopleTeam = new();

                peopleEquip.ForEach(personEquip =>
                {
                    if (personEquip.Name != personComplete.Name)
                        newPeopleTeam.Add(personEquip);
                });
                
                if (newPeopleTeam.Count < 1)
                {
                    TempData["error"] = "Pessoa - Essa pessoa é a única na equipe. Exclua a equipe para consegui excluir essa pessoa.";

                    return RedirectToAction(nameof(Index));
                }

                equip.People = newPeopleTeam;

                var equipInsertion = await new ConnectToEquipApi().EditEquip(equip);

                if (equipInsertion == null || equipInsertion.Error != "")
                {
                    TempData["error"] = "Pessoa - Houve um erro na edição da equipe com o nome da pessoa. Favor tentar novamente.";

                    return RedirectToAction(nameof(Index));
                }
            }

            var personRemove = await new ConnectToPersonApi().RemovePerson(id);

            if (personRemove.Error != "ok")
            {
                TempData["error"] = "Houve um erro na exclusão da pessoa. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
