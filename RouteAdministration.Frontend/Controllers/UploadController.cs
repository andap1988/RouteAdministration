using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RouteAdministration.Frontend.Service;
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
           /* var name = HttpContext.Session.GetString("username");

            if (name == null)
                return RedirectToRoute(new { controller = "Home", action = "Index" });*/

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

            return View(ViewData);
        }
    }
}
