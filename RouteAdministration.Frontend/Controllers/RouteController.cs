using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Models;
using RouteAdministration.Frontend.Service;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RouteAdministration.Frontend.Controllers
{
    [Authorize]
    public class RouteController : Controller
    {
        IWebHostEnvironment _appEnvironment;
        private readonly RARouteService _raRouteService;

        public RouteController(IWebHostEnvironment env, RARouteService raRouteService)
        {
            _appEnvironment = env;
            _raRouteService = raRouteService;
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

            var folder = _appEnvironment.WebRootPath + "\\File\\";
            string[] files = Directory.GetFiles(folder);
            bool hasPlan = false;

            foreach (var file in files)
            {
                var split = file.Split("\\");
                var name = split[split.Length - 1];

                if (name == "Plan.xlsx")
                    hasPlan = true;
            }

            if (!hasPlan)
            {
                TempData["error"] = "Não foi carregado nenhum arquivo (.xlsx) para leitura.";

                return RedirectToRoute(new { controller = "Upload", action = "Index" });
            }

            var equips = await new ConnectToEquipApi().GetEquips();

            if (equips.Count < 1)
            {
                TempData["error"] = "Não é possível iniciar uma rota sem ter pelo menos 1 (uma) equipe cadastrada.";

                return RedirectToRoute(new { controller = "Home", action = "Index" });
            }

            var headers = ReadFiles.ReadHeaderExcelFile(_appEnvironment.WebRootPath);

            ViewBag.User = user;
            ViewBag.Authenticate = authenticate;
            ViewBag.Headers = headers;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Services(List<string> selectedHeaders)
        {
            ViewBag.User = HttpContext.User.Identity.Name;
            ViewBag.Authenticate = true;

            if (HttpContext.User.IsInRole("adm"))
                ViewBag.Role = "adm";
            else
                ViewBag.Role = "user";

            if (!selectedHeaders.Contains("OS") ||
                !selectedHeaders.Contains("CIDADE") ||
                !selectedHeaders.Contains("BASE") ||
                !selectedHeaders.Contains("SERVIÇO") ||
                !selectedHeaders.Contains("ENDEREÇO") ||
                !selectedHeaders.Contains("NUMERO") ||
                !selectedHeaders.Contains("COMPLEMENTO") ||
                !selectedHeaders.Contains("CEP") ||
                !selectedHeaders.Contains("BAIRRO"))
            {
                TempData["error"] = "As colunas: OS, CIDADE, BASE, SERVIÇO, ENDEREÇO, NUMERO, COMPLEMENTO, CEP E BAIRRO são obrigatórias.";

                return RedirectToAction(nameof(Index));
            }

            List<string> services = new();
            List<string> servicesSearch = new();

            selectedHeaders.ForEach(header =>
            {
                if (header.ToUpper() == "SERVIÇO" || header.ToUpper() == "SERVICO")
                {
                    services = ReadFiles.ReadColumnExcelFile(_appEnvironment.WebRootPath, header);
                }
            });

            services.Sort((x, y) => x.CompareTo(y));

            var servicesDuplicate = services.GroupBy(service => service.ToString()).Where(x => x.Count() > 1);

            foreach (var service in servicesDuplicate)
            {
                servicesSearch.Add(service.Key);
            }

            var a = ReadFiles.ReadExcelFile(selectedHeaders, _appEnvironment.WebRootPath);

            WriteFiles.WriteStringInFolder(selectedHeaders, null, "headers", _appEnvironment.WebRootPath);

            ViewBag.Services = servicesSearch;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cities(string service)
        {
            ViewBag.User = HttpContext.User.Identity.Name;
            ViewBag.Authenticate = true;

            if (HttpContext.User.IsInRole("adm"))
                ViewBag.Role = "adm";
            else
                ViewBag.Role = "user";

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

            dictonaryByService.ForEach(dict =>
            {
                cities.Add(dict["CIDADE"]);
            });

            cities = cities.Distinct().ToList();

            cities.Sort((cityOne, cityTwo) => cityOne.CompareTo(cityTwo));

            ViewBag.Cities = cities;
            ViewBag.Service = service;

            WriteFiles.WriteStringInFolder(null, service, "service", _appEnvironment.WebRootPath);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Equips(string city)
        {
            ViewBag.User = HttpContext.User.Identity.Name;
            ViewBag.Authenticate = true;

            if (HttpContext.User.IsInRole("adm"))
                ViewBag.Role = "adm";
            else
                ViewBag.Role = "user";

            var service = ReadFiles.ReadFileStringInFolder("service", _appEnvironment.WebRootPath);
            var columns = ReadFiles.ReadFileInFolder("headers", _appEnvironment.WebRootPath);
            var dataBySelectedHeaders = ReadFiles.ReadExcelFile(columns, _appEnvironment.WebRootPath);

            List<string> cities = new();
            List<IDictionary<string, string>> dictonaryByServiceAndCity = new();

            columns.ForEach(column =>
            {
                if (column.ToUpper() == "SERVIÇO")
                {
                    for (int i = 0; i < dataBySelectedHeaders.Count; i++)
                    {
                        dictonaryByServiceAndCity = dataBySelectedHeaders
                            .Where(data => data.ContainsKey(column))
                            .Where(data => data.Values.Contains(service))
                            .Where(data => data.Values.Contains(city))
                            .ToList();
                    }
                }
            });

            var quantityService = dictonaryByServiceAndCity.Count;

            var listEquipsByCity = await new ConnectToEquipApi().GetEquipByCity(city);

            if (listEquipsByCity == null || listEquipsByCity[0].Error != "")
            {
                TempData["error"] = "Equipe - A API está fora do ar. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            WriteFiles.WriteStringInFolder(null, city, "city", _appEnvironment.WebRootPath);

            ViewBag.City = city;
            ViewBag.QuantityService = quantityService;

            return View(listEquipsByCity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GenerateRoute(List<string> selectedEquips)
        {
            ViewBag.User = HttpContext.User.Identity.Name;
            ViewBag.Authenticate = true;

            if (HttpContext.User.IsInRole("adm"))
                ViewBag.Role = "adm";
            else
                ViewBag.Role = "user";

            if (selectedEquips.Count < 1)
            {
                TempData["error"] = "Equipe - É necessário adicionar pelo menos uma equipe para gerar a rota.";

                return RedirectToAction(nameof(Index));
            }

            List<Equip> listEquipsByEquipName = new();

            listEquipsByEquipName = await new ConnectToEquipApi().GetEquipsByEquipsName(selectedEquips);

            if (listEquipsByEquipName == null || listEquipsByEquipName[0].Error != "")
            {
                TempData["error"] = "Equipe - A API está fora do ar. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            var service = ReadFiles.ReadFileStringInFolder("service", _appEnvironment.WebRootPath);
            var city = ReadFiles.ReadFileStringInFolder("city", _appEnvironment.WebRootPath);
            var columns = ReadFiles.ReadFileInFolder("headers", _appEnvironment.WebRootPath);
            var dataBySelectedHeaders = ReadFiles.ReadExcelFile(columns, _appEnvironment.WebRootPath);

            List<string> cities = new();
            List<IDictionary<string, string>> dictonaryByServiceAndCity = new();

            columns.ForEach(column =>
            {
                if (column.ToUpper() == "SERVIÇO")
                {
                    for (int i = 0; i < dataBySelectedHeaders.Count; i++)
                    {
                        dictonaryByServiceAndCity = dataBySelectedHeaders
                            .Where(data => data.ContainsKey(column))
                            .Where(data => data.Values.Contains(service))
                            .Where(data => data.Values.Contains(city))
                            .ToList();
                    }
                }
            });

            List<string> otherColumns = new();

            columns.ForEach(column =>
            {
                if (column != "OS" && column != "CIDADE" && column != "BASE" && column != "SERVIÇO" && column != "ENDEREÇO"
                    && column != "NUMERO" && column != "COMPLEMENTO" && column != "CEP" && column != "BAIRRO")
                {
                    otherColumns.Add(column);
                }
            });

            if ((dictonaryByServiceAndCity.Count / selectedEquips.Count) > 5)
            {
                TempData["error"] = "A quantidade de serviços por equipe supera a marca de 5. A rota não será gerada.";

                return RedirectToAction(nameof(Index));
            }
            if (dictonaryByServiceAndCity.Count < selectedEquips.Count)
            {
                TempData["error"] = "A quantidade de equipes selecionadas é maior que a quantidade de serviço disponível. A rota não será gerada.";

                return RedirectToAction(nameof(Index));
            }

            /* Begin Header DOC */

            Document document = new();

            Section section = document.AddSection();

            Paragraph emptyParagraph = section.AddParagraph();
            emptyParagraph.AppendText(" ");
            emptyParagraph = section.AddParagraph();
            emptyParagraph.AppendText(" ");

            Paragraph paragraphTitle = section.AddParagraph();
            TextRange trTitle = paragraphTitle.AppendText($"ROTA DE TRABALHO - {DateTime.Now.Date.ToString("dd/MM/yyyy")}");
            trTitle.CharacterFormat.FontSize = 23;
            trTitle.CharacterFormat.Bold = true;
            trTitle.CharacterFormat.FontName = "Arial";
            paragraphTitle.Format.HorizontalAlignment = HorizontalAlignment.Center;
            paragraphTitle.Format.LineSpacing = 15;

            Paragraph paragraphDate = section.AddParagraph();
            TextRange trDate = paragraphDate.AppendText($"{service}");
            trDate.CharacterFormat.FontSize = 17;
            trDate.CharacterFormat.Bold = true;
            trDate.CharacterFormat.FontName = "Arial";
            paragraphDate.Format.HorizontalAlignment = HorizontalAlignment.Center;

            emptyParagraph = section.AddParagraph();
            emptyParagraph.AppendText(" ");
            emptyParagraph = section.AddParagraph();
            emptyParagraph.AppendText(" ");

            /* End Header DOC */

            /* Begin Equip DOC */

            if (listEquipsByEquipName.Count < 2)
            {
                Paragraph paragraphEquip = section.AddParagraph();
                TextRange trEquip = paragraphEquip.AppendText($"Nome da Equipe: {listEquipsByEquipName[0].Name}");
                trEquip.CharacterFormat.FontSize = 14;
                trEquip.CharacterFormat.Bold = true;
                trEquip.CharacterFormat.FontName = "Arial";

                emptyParagraph = section.AddParagraph();
                emptyParagraph.AppendText(" ");

                dictonaryByServiceAndCity.ForEach(dictionary =>
                {
                    Paragraph paragraph1 = section.AddParagraph();
                    TextRange tr1 = paragraph1.AppendText($"Contrato: {(dictionary.ContainsKey("CONTRATO") ? dictionary["CONTRATO"] : "                     ")} - Assinante: {(dictionary.ContainsKey("ASSINANTE") ? dictionary["ASSINANTE"] : "                            ")} - Período:     :     /     :     .");
                    tr1.CharacterFormat.FontSize = 12;
                    tr1.CharacterFormat.Bold = true;
                    tr1.CharacterFormat.FontName = "Arial";
                    tr1.CharacterFormat.UnderlineStyle = UnderlineStyle.Single;
                    paragraph1.Format.LineSpacing = 15;

                    Paragraph paragraph2 = section.AddParagraph();
                    TextRange tr2 = paragraph2.AppendText($"Endereço: {dictionary["ENDEREÇO"]}, {dictionary["NUMERO"]}, {dictionary["BAIRRO"]}, {dictionary["CIDADE"]}, CEP: {dictionary["CEP"]}  - {(dictionary.ContainsKey("TELEFONE 1") ? dictionary["TELEFONE 1"] : "")}");
                    tr2.CharacterFormat.FontName = "Arial";
                    paragraph2.Format.LineSpacing = 15;

                    Paragraph paragraph3 = section.AddParagraph();
                    TextRange tr3 = paragraph3.AppendText($"O.S.:{dictionary["OS"]} - ");
                    tr3.CharacterFormat.FontName = "Arial";

                    tr3 = paragraph3.AppendText($"Tipo OS: {(dictionary.ContainsKey("TIPO OS") ? dictionary["TIPO OS"] : "_______________")}");
                    tr3.CharacterFormat.TextColor = Color.White;
                    tr3.CharacterFormat.TextBackgroundColor = Color.Red;
                    tr3.CharacterFormat.FontName = "Arial";
                    paragraph3.Format.LineSpacing = 15;

                    if (otherColumns.Count > 0)
                    {
                        for (int col = 0; col < otherColumns.Count; col++)
                        {
                            Paragraph paragraph4 = section.AddParagraph();
                            TextRange tr4 = paragraph4.AppendText($"{otherColumns[col]}: {dictionary[otherColumns[col]]}");
                            tr4.CharacterFormat.FontName = "Arial";
                            paragraph4.Format.LineSpacing = 15;
                        }
                    }

                    emptyParagraph = section.AddParagraph();
                    emptyParagraph.AppendText(" ");
                });
            }
            else if (listEquipsByEquipName.Count == dictonaryByServiceAndCity.Count)
            {
                var i = 0;

                dictonaryByServiceAndCity.ForEach(dictionary =>
                {
                    Paragraph paragraphEquip = section.AddParagraph();
                    TextRange trEquip = paragraphEquip.AppendText($"Nome da Equipe: {listEquipsByEquipName[i].Name}");
                    trEquip.CharacterFormat.FontSize = 14;
                    trEquip.CharacterFormat.Bold = true;
                    trEquip.CharacterFormat.FontName = "Arial";

                    emptyParagraph = section.AddParagraph();
                    emptyParagraph.AppendText(" ");

                    Paragraph paragraph1 = section.AddParagraph();
                    TextRange tr1 = paragraph1.AppendText($"Contrato: {(dictionary.ContainsKey("CONTRATO") ? dictionary["CONTRATO"] : "                     ")} - Assinante: {(dictionary.ContainsKey("ASSINANTE") ? dictionary["ASSINANTE"] : "                            ")} - Período:     :     /     :     .");
                    tr1.CharacterFormat.FontSize = 12;
                    tr1.CharacterFormat.Bold = true;
                    tr1.CharacterFormat.FontName = "Arial";
                    tr1.CharacterFormat.UnderlineStyle = UnderlineStyle.Single;
                    paragraph1.Format.LineSpacing = 15;

                    Paragraph paragraph2 = section.AddParagraph();
                    TextRange tr2 = paragraph2.AppendText($"Endereço: {dictionary["ENDEREÇO"]}, {dictionary["NUMERO"]}, {dictionary["BAIRRO"]}, {dictionary["CIDADE"]}, CEP: {dictionary["CEP"]}  - {(dictionary.ContainsKey("TELEFONE 1") ? dictionary["TELEFONE 1"] : "")}");
                    tr2.CharacterFormat.FontName = "Arial";
                    paragraph2.Format.LineSpacing = 15;

                    Paragraph paragraph3 = section.AddParagraph();
                    TextRange tr3 = paragraph3.AppendText($"O.S.:{dictionary["OS"]} - ");
                    tr3.CharacterFormat.FontName = "Arial";

                    tr3 = paragraph3.AppendText($"Tipo OS: {(dictionary.ContainsKey("TIPO OS") ? dictionary["TIPO OS"] : "_______________")}");
                    tr3.CharacterFormat.TextColor = Color.White;
                    tr3.CharacterFormat.TextBackgroundColor = Color.Red;
                    tr3.CharacterFormat.FontName = "Arial";
                    paragraph3.Format.LineSpacing = 15;

                    if (otherColumns.Count > 0)
                    {
                        for (int col = 0; col < otherColumns.Count; col++)
                        {
                            Paragraph paragraph4 = section.AddParagraph();
                            TextRange tr4 = paragraph4.AppendText($"{otherColumns[col]}: {dictionary[otherColumns[col]]}");
                            tr4.CharacterFormat.FontName = "Arial";
                            paragraph4.Format.LineSpacing = 15;
                        }
                    }

                    emptyParagraph = section.AddParagraph();
                    emptyParagraph.AppendText(" ");

                    i++;
                });
            }
            else if (listEquipsByEquipName.Count > 1)
            {
                int serviceSplit = dictonaryByServiceAndCity.Count / listEquipsByEquipName.Count;
                decimal restSplit = dictonaryByServiceAndCity.Count % listEquipsByEquipName.Count;
                int listEquip = 0;

                if (restSplit > 5)
                {
                    serviceSplit = 5;
                    restSplit = Math.Ceiling((decimal)dictonaryByServiceAndCity.Count % 5);
                }

                for (int k = 0; k < dictonaryByServiceAndCity.Count; k++)
                {
                    Paragraph paragraphEquip = section.AddParagraph();
                    TextRange trEquip = paragraphEquip.AppendText($"Nome da Equipe: {listEquipsByEquipName[listEquip].Name}");
                    trEquip.CharacterFormat.FontSize = 14;
                    trEquip.CharacterFormat.Bold = true;
                    trEquip.CharacterFormat.FontName = "Arial";

                    emptyParagraph = section.AddParagraph();
                    emptyParagraph.AppendText(" ");

                    listEquip++;

                    for (int i = 0; i < serviceSplit; i++)
                    {
                        var dictionary = dictonaryByServiceAndCity[k];

                        Paragraph paragraph1 = section.AddParagraph();
                        TextRange tr1 = paragraph1.AppendText($"Contrato: {(dictionary.ContainsKey("CONTRATO") ? dictionary["CONTRATO"] : "                     ")} - Assinante: {(dictionary.ContainsKey("ASSINANTE") ? dictionary["ASSINANTE"] : "                            ")} - Período:     :     /     :     .");
                        tr1.CharacterFormat.FontSize = 12;
                        tr1.CharacterFormat.Bold = true;
                        tr1.CharacterFormat.FontName = "Arial";
                        tr1.CharacterFormat.UnderlineStyle = UnderlineStyle.Single;
                        paragraph1.Format.LineSpacing = 15;

                        Paragraph paragraph2 = section.AddParagraph();
                        TextRange tr2 = paragraph2.AppendText($"Endereço: {dictionary["ENDEREÇO"]}, {dictionary["NUMERO"]}, {dictionary["BAIRRO"]}, {dictionary["CIDADE"]}, CEP: {dictionary["CEP"]}  - {(dictionary.ContainsKey("TELEFONE 1") ? dictionary["TELEFONE 1"] : "")}");
                        tr2.CharacterFormat.FontName = "Arial";
                        paragraph2.Format.LineSpacing = 15;

                        Paragraph paragraph3 = section.AddParagraph();
                        TextRange tr3 = paragraph3.AppendText($"O.S.:{dictionary["OS"]} - ");
                        tr3.CharacterFormat.FontName = "Arial";

                        tr3 = paragraph3.AppendText($"Tipo OS: {(dictionary.ContainsKey("TIPO OS") ? dictionary["TIPO OS"] : "_______________")}");
                        tr3.CharacterFormat.TextColor = Color.White;
                        tr3.CharacterFormat.TextBackgroundColor = Color.Red;
                        tr3.CharacterFormat.FontName = "Arial";
                        paragraph3.Format.LineSpacing = 15;

                        if (otherColumns.Count > 0)
                        {
                            for (int col = 0; col < otherColumns.Count; col++)
                            {
                                Paragraph paragraph4 = section.AddParagraph();
                                TextRange tr4 = paragraph4.AppendText($"{otherColumns[col]}: {dictionary[otherColumns[col]]}");
                                tr4.CharacterFormat.FontName = "Arial";
                                paragraph4.Format.LineSpacing = 15;
                            }
                        }

                        emptyParagraph = section.AddParagraph();
                        emptyParagraph.AppendText(" ");

                        k++;
                    }

                    k--;

                    if (restSplit == dictonaryByServiceAndCity.Count - (k + 1) && restSplit != 0)
                    {
                        if (!(listEquipsByEquipName.Count == listEquip))
                        {
                            Paragraph paragraphEquipRest = section.AddParagraph();
                            TextRange trEquipRest = paragraphEquipRest.AppendText($"Nome da Equipe: {listEquipsByEquipName[listEquip].Name}");
                            trEquipRest.CharacterFormat.FontSize = 14;
                            trEquipRest.CharacterFormat.Bold = true;
                            trEquipRest.CharacterFormat.FontName = "Arial";

                            emptyParagraph = section.AddParagraph();
                            emptyParagraph.AppendText(" ");
                        }

                        for (int i = 0; i < restSplit; i++)
                        {
                            var dictionary = dictonaryByServiceAndCity[k];

                            Paragraph paragraph1 = section.AddParagraph();
                            TextRange tr1 = paragraph1.AppendText($"Contrato: {(dictionary.ContainsKey("CONTRATO") ? dictionary["CONTRATO"] : "                     ")} - Assinante: {(dictionary.ContainsKey("ASSINANTE") ? dictionary["ASSINANTE"] : "                            ")} - Período:     :     /     :     .");
                            tr1.CharacterFormat.FontSize = 12;
                            tr1.CharacterFormat.Bold = true;
                            tr1.CharacterFormat.FontName = "Arial";
                            tr1.CharacterFormat.UnderlineStyle = UnderlineStyle.Single;
                            paragraph1.Format.LineSpacing = 15;

                            Paragraph paragraph2 = section.AddParagraph();
                            TextRange tr2 = paragraph2.AppendText($"Endereço: {dictionary["ENDEREÇO"]}, {dictionary["NUMERO"]}, {dictionary["BAIRRO"]}, {dictionary["CIDADE"]}, CEP: {dictionary["CEP"]}  - {(dictionary.ContainsKey("TELEFONE 1") ? dictionary["TELEFONE 1"] : "")}");
                            tr2.CharacterFormat.FontName = "Arial";
                            paragraph2.Format.LineSpacing = 15;

                            Paragraph paragraph3 = section.AddParagraph();
                            TextRange tr3 = paragraph3.AppendText($"O.S.:{dictionary["OS"]} - ");
                            tr3.CharacterFormat.FontName = "Arial";

                            tr3 = paragraph3.AppendText($"Tipo OS: {(dictionary.ContainsKey("TIPO OS") ? dictionary["TIPO OS"] : "_______________")}");
                            tr3.CharacterFormat.TextColor = Color.White;
                            tr3.CharacterFormat.TextBackgroundColor = Color.Red;
                            tr3.CharacterFormat.FontName = "Arial";
                            paragraph3.Format.LineSpacing = 15;

                            if (otherColumns.Count > 0)
                            {
                                for (int col = 0; col < otherColumns.Count; col++)
                                {
                                    Paragraph paragraph4 = section.AddParagraph();
                                    TextRange tr4 = paragraph4.AppendText($"{otherColumns[col]}: {dictionary[otherColumns[col]]}");
                                    tr4.CharacterFormat.FontName = "Arial";
                                    paragraph4.Format.LineSpacing = 15;
                                }
                            }

                            emptyParagraph = section.AddParagraph();
                            emptyParagraph.AppendText(" ");

                            k++;
                        }
                    }
                }
            }

            service = service.Replace("Ç", "C").Replace("ç", "c").Replace("Ã", "A").Replace("ã", "a").Replace("É", "e").Replace("é", "e").Replace(" ", "");
            city = city.Replace("Ç", "C").Replace("ç", "c").Replace("Ã", "A").Replace("ã", "a").Replace("É", "e").Replace("é", "e").Replace(" ", "");

            string nameFile = $"Route{service}{city}{DateTime.Now.Date.ToString("ddMMyyyy")}.docx";

            document.SaveToFile(_appEnvironment.WebRootPath + "\\File\\" + nameFile, FileFormat.Docx);

            /* FIM DOC*/

            HistoryGenerateFile historyGenerateFile = new();
            historyGenerateFile.Date = DateTime.Now.Date.ToString("dd/MM/yyyy");
            historyGenerateFile.Service = service;
            historyGenerateFile.City = city;
            historyGenerateFile.Headers = columns;
            historyGenerateFile.Equips = selectedEquips;
            historyGenerateFile.FileName = nameFile;
            historyGenerateFile.FullPath = _appEnvironment.WebRootPath + "\\File\\" + nameFile;

            _raRouteService.Create(historyGenerateFile);

            RemoveFiles.RemoveFromFolder("headers", ".txt", _appEnvironment.WebRootPath);
            RemoveFiles.RemoveFromFolder("service", ".txt", _appEnvironment.WebRootPath);
            RemoveFiles.RemoveFromFolder("city", ".txt", _appEnvironment.WebRootPath);

            return View();
        }
    }
}
