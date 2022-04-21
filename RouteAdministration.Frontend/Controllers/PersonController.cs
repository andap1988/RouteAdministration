using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using RouteAdministration.Frontend.Service;
using System.Threading.Tasks;

namespace RouteAdministration.Frontend.Controllers
{
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

            return View(people);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Person>> Create(Person person)
        {
            var personInsertion = await new ConnectToPersonApi().CreateNewPerson(person);

            if (personInsertion == null)
                return BadRequest("Pessoa - Houve um erro na gravação da nova pessoa. Favor tentar novamente");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await new ConnectToPersonApi().GetPersonById(id);

            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Person person)
        {
            var personEdit = await new ConnectToPersonApi().EditPerson(person);

            if (personEdit == null)
                return BadRequest("Pessoa - Houve um erro na edição da pessoa. Favor tentar novamente");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await new ConnectToPersonApi().GetPersonById(id);

            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, Person person)
        {
            var personRemove = await new ConnectToPersonApi().RemovePerson(id);

            if (personRemove.Error != "ok")
                return BadRequest("Pessoa - Houve um erro na exclusão do usuário. Favor tentar novamente");

            return RedirectToAction(nameof(Index));
        }
    }
}
