using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using RouteAdministration.Frontend.Service;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RouteAdministration.Frontend.Controllers
{
    public class UploadController : Controller
    {
        IWebHostEnvironment _appEnvironment;

        public UploadController(IWebHostEnvironment env)
        {
            _appEnvironment = env;
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendFile(IFormFile file)
        {
            if (ReadFiles.IsValid("Plan", ".xlsx", _appEnvironment.WebRootPath))
                RemoveFiles.RemoveFromFolder("Plan", ".xlsx", _appEnvironment.WebRootPath);

            var pathFile = Path.GetTempFileName();
            string pathWebRoot = _appEnvironment.WebRootPath;

            if (CheckExcelFile.IsExcel(file))
            {
                if (!await WriteFiles.WriteFileInFolder(file, pathWebRoot))
                    return BadRequest(new { message = "Houve um erro na gravação do arquivo. Por favor, tente novamente." });
            }
            else
            {
                return BadRequest(new { message = "Apenas arquivos com extensão .xls ou .xlsx" });
            }

            ViewData["Resultado"] = $"Um arquivo foi enviado ao servidor, com tamanho total de {file.Length} bytes!";

            ReadFiles.ReOrderExcel("Plan", ".xlsx", _appEnvironment.WebRootPath);

            var headers = ReadFiles.ReadHeaderExcelFile(pathWebRoot);

            List<string> cities = new();
            List<string> uniqueCities = new();

            headers.ForEach(header =>
            {
                if (header == "CIDADE")
                {
                    cities = ReadFiles.ReadColumnExcelFile(pathWebRoot, header);
                }
            });

            cities.ForEach(city =>
            {
                if (!uniqueCities.Contains(city))
                    uniqueCities.Add(city);
            });

            foreach(var city in uniqueCities)
            {
                City newCity = new() { Name = city, State = "SP" };

                await new ConnectToCityApi().CreateNewCity(newCity);
            }

            return View(ViewData);
        }
    }
}
