using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using RouteAdministration.Frontend.Service;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RouteAdministration.Frontend.Controllers
{
    public class RouteController : Controller
    {
        IWebHostEnvironment _appEnvironment;

        public RouteController(IWebHostEnvironment env)
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

            var headers = ReadFiles.ReadHeaderExcelFile(_appEnvironment.WebRootPath);


            ViewBag.User = user;
            ViewBag.Authenticate = authenticate;
            ViewBag.Headers = headers;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ServicesCities(List<string> selectedHeaders)
        {
            List<string> services = new();
            List<string> servicesSearch = new();

            selectedHeaders.ForEach(header =>
            {
                if (header.ToUpper() == "SERVIÇO" || header.ToUpper() == "SERVICO")
                {
                    services = ReadFiles.ReadServiceExcelFile(_appEnvironment.WebRootPath, header);
                }
            });

            services.Sort((x, y) => x.CompareTo(y));

            var servicesDuplicate = services.GroupBy(service => service.ToString()).Where(x => x.Count() > 1);

            foreach (var service in servicesDuplicate)
            {
                servicesSearch.Add(service.Key);
            }

            var a = ReadFiles.ReadExcelFile(selectedHeaders, _appEnvironment.WebRootPath);

            WriteFiles.WriteStringInFolder(selectedHeaders, "headers", _appEnvironment.WebRootPath);
            /*WriteFiles.WriteStringInFolder(services, "services", _appEnvironment.WebRootPath);
            WriteFiles.WriteStringInFolder(servicesSearch, "servicesSearch", _appEnvironment.WebRootPath);*/


            ViewBag.Services = servicesSearch;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Cities(string service)
        {
            var columns = ReadFiles.ReadFileInFolder("headers", _appEnvironment.WebRootPath);
            var dataBySelectedHeaders = ReadFiles.ReadExcelFile(columns, _appEnvironment.WebRootPath);

            List<string> cities = new();
            List<IDictionary<string, string>> dictonaryByService = new();

            columns.ForEach(column =>
            {
                if (column.ToUpper() == "CIDADE")
                {
                    for (int i = 0; i < dataBySelectedHeaders.Count; i++)
                    {
                        dictonaryByService = dataBySelectedHeaders.Where(data => data.ContainsKey(column)).Where(data => data.Values.Contains(service)).ToList();
                    }
                }
            });

            dictonaryByService.ForEach(dictonary =>
            {
                cities.Add(dictonary.Values.FirstOrDefault());
            });

            cities = cities.Distinct().ToList();

            ViewBag.Cities = cities;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Equips(string cities)
        {

            var a = 10;

            /*var columns = ReadFiles.ReadFileInFolder("headers", _appEnvironment.WebRootPath);
            var dataBySelectedHeaders = ReadFiles.ReadExcelFile(columns, _appEnvironment.WebRootPath);

            List<string> cities = new();
            List<IDictionary<string, string>> dictonaryByService = new();

            columns.ForEach(column =>
            {
                if (column.ToUpper() == "CIDADE")
                {
                    for (int i = 0; i < dataBySelectedHeaders.Count; i++)
                    {
                        dictonaryByService = dataBySelectedHeaders.Where(data => data.ContainsKey(column)).Where(data => data.Values.Contains(service)).ToList();
                    }
                }
            });

            dictonaryByService.ForEach(dictonary =>
            {
                cities.Add(dictonary.Values.FirstOrDefault());
            });

            cities = cities.Distinct().ToList();

            ViewBag.Cities = cities;*/

            return View();
        }

    }
}
