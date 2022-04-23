using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Models;
using RouteAdministration.Frontend.Service;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RouteAdministration.Frontend.Controllers
{
    [Authorize]
    public class DownloadController : Controller
    {
        IWebHostEnvironment _appEnvironment;

        public DownloadController(IWebHostEnvironment env)
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

            var folder = _appEnvironment.WebRootPath + "\\File\\";

            string[] files = Directory.GetFiles(folder);

            List<HistoryGenerateFile> hgFiles = new();

            foreach (var file in files)
            {
                var split = file.Split("\\");
                var name = split[split.Length - 1];

                var extesion = name.Split(".")[1];

                if (extesion == "docx")
                    hgFiles.Add(new HistoryGenerateFile { FileName = name, FullPath = file });
            }

            return View(hgFiles);
        }

        public FileResult DownloadFile(string fileName)
        {
            var folder = _appEnvironment.WebRootPath + "\\File\\";
            var pathFinal = folder + fileName;
            byte[] bytes = System.IO.File.ReadAllBytes(pathFinal);
            string contentType = "application/octet-stream";

            return File(bytes, contentType, fileName);
        }
    }
}
