using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace RouteAdministration.Frontend.Controllers
{
    public class LoginController : Controller
    {
        IWebHostEnvironment _appEnvironment;
        private readonly RARouteService _raRouteService;

        public LoginController(IWebHostEnvironment env, RARouteService raRouteService)
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

            List<HistoryGenerateFile> hgFiles = new();

            foreach (var file in files)
            {
                var split = file.Split("\\");
                var name = split[split.Length - 1];

                var extesion = name.Split(".")[1];

                if (extesion == "docx")
                    hgFiles.Add(new HistoryGenerateFile { FileName = name, FullPath = file });
            }

            List<User> users = new();

            users = await new ConnectToUserApi().GetUsers();

            if (users == null || users[0].Error != "")
            {
                TempData["error"] = "Usuário - A API está fora do ar. Tente novamente.";

                return RedirectToAction("Index", "Home");
            }

            users.ForEach(user =>
            {
                user.Password = "";
            });

            ViewBag.Files = hgFiles;
            ViewBag.Users = users;
            ViewBag.User = user;
            ViewBag.Authenticate = authenticate;

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<User>> Login(User userLogin)
        {
            if (string.IsNullOrEmpty(userLogin.Username))
            {
                TempData["error"] = "O nome do usuário é obrigatório.";

                return RedirectToAction(nameof(Index));
            }
            else if (string.IsNullOrEmpty(userLogin.Password))
            {
                TempData["error"] = "A senha do usuário é obrigatório.";

                return RedirectToAction(nameof(Index));
            }

            var userLoginSearch = await new ConnectToUserApi().GetUserByUsername(userLogin.Username);

            if (userLoginSearch == null || userLoginSearch.Error != "")
            {
                TempData["error"] = "Login - A API está fora do ar. Tente novamente.";

                return RedirectToAction(nameof(Index));
            }

            if (userLoginSearch != null)
            {
                if (userLogin.Password == userLoginSearch.Password)
                {
                    List<Claim> userClaims = new()
                    {
                        new Claim(ClaimTypes.Name, userLoginSearch.Username),
                        new Claim("Role", userLoginSearch.Role),
                        new Claim(ClaimTypes.Role, userLoginSearch.Role),
                    };

                    var myIdentity = new ClaimsIdentity(userClaims, "User");
                    var userPrincipal = new ClaimsPrincipal(new[] { myIdentity });

                    await HttpContext.SignInAsync(userPrincipal);

                    if (userLoginSearch.Username == "temp")
                    {
                        ViewBag.User = "temp";

                        return RedirectToAction(nameof(EditTemp));
                    }

                    TempData["success"] = "Usuário logado com sucesso!";

                    return RedirectToRoute(new { controller = "Upload", action = "Index" });
                }
            }

            TempData["error"] = "Usuário ou senha inválido.";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult EditTemp()
        {
            string user = "Anonymous";

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                user = HttpContext.User.Identity.Name;

                if (HttpContext.User.IsInRole("adm"))
                    ViewBag.Role = "adm";
                else
                    ViewBag.Role = "user";
            }
            else
            {
                user = "Não Logado";
                ViewBag.Role = "";
            }

            ViewBag.User = user;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutTemp(User userLogin)
        {
            if (string.IsNullOrEmpty(userLogin.Name) || string.IsNullOrWhiteSpace(userLogin.Name))
            {
                TempData["error"] = "O nome do usuário não pode ser vazio.";

                return RedirectToAction(nameof(EditTemp));
            }
            else if (string.IsNullOrEmpty(userLogin.Username) || string.IsNullOrWhiteSpace(userLogin.Username))
            {
                TempData["error"] = "O usuário não pode ser vazio.";

                return RedirectToAction(nameof(EditTemp));
            }
            else if (string.IsNullOrEmpty(userLogin.Password) || string.IsNullOrWhiteSpace(userLogin.Password))
            {
                TempData["error"] = "A senha não pode ser vazia.";

                return RedirectToAction(nameof(EditTemp));
            }

            var userLoginSearch = await new ConnectToUserApi().GetUserByUsername(userLogin.Username);

            if (userLoginSearch != null)
            {
                TempData["error"] = "Já existe um usuário cadastrado com o usuário informado.";

                return RedirectToAction(nameof(Index));
            }

            userLogin.Role = "adm";

            var userLoginInsertion = await new ConnectToUserApi().CreateNewUser(userLogin);

            if (userLoginInsertion == null || userLoginInsertion.Error != "")
            {
                TempData["error"] = "Usuário - Houve um erro na gravação do novo usuário. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            await HttpContext.SignOutAsync();

            var userRemove = await new ConnectToUserApi().GetUserByUsername("temp");
            var userRemoved = await new ConnectToUserApi().RemoveUser(userRemove.Id);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
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
        public async Task<ActionResult<User>> Create(User userLogin)
        {
            if (string.IsNullOrEmpty(userLogin.Name) || string.IsNullOrWhiteSpace(userLogin.Name))
            {
                TempData["error"] = "O nome do usuário não pode ser vazio.";

                return RedirectToAction(nameof(Index));
            }
            else if (string.IsNullOrEmpty(userLogin.Username) || string.IsNullOrWhiteSpace(userLogin.Username))
            {
                TempData["error"] = "O usuário não pode ser vazio.";

                return RedirectToAction(nameof(Index));
            }
            else if (string.IsNullOrEmpty(userLogin.Password) || string.IsNullOrWhiteSpace(userLogin.Password))
            {
                TempData["error"] = "A senha não pode ser vazia.";

                return RedirectToAction(nameof(Index));
            }
            else if (userLogin.Username == "temp")
            {
                TempData["error"] = "Não é possível criar uma conta com esse usuário.";

                return RedirectToAction(nameof(Index));
            }

            var userLoginSearch = await new ConnectToUserApi().GetUserByUsername(userLogin.Username);

            if (userLoginSearch != null)
            {
                TempData["error"] = "Já existe um usuário cadastrado com o usuário informado.";

                return RedirectToAction(nameof(Index));
            }

            var userLoginInsertion = await new ConnectToUserApi().CreateNewUser(userLogin);

            if (userLoginInsertion == null || userLoginInsertion.Error != "")
            {
                TempData["error"] = "Usuário - Houve um erro na gravação do novo usuário. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            TempData["success"] = "Usuário cadastrado com sucesso!";

            return RedirectToAction(nameof(Index));
        }

        public FileResult DownloadFile(string fileName)
        {
            var folder = _appEnvironment.WebRootPath + "\\File\\";
            var pathFinal = folder + fileName;
            byte[] bytes = System.IO.File.ReadAllBytes(pathFinal);
            string contentType = "application/octet-stream";

            return File(bytes, contentType, fileName);
        }

        public IActionResult DeleteFile(string id)
        {
            ViewBag.User = HttpContext.User.Identity.Name;

            if (HttpContext.User.IsInRole("adm"))
                ViewBag.Role = "adm";
            else
                ViewBag.Role = "user";

            if (id == null)
            {
                TempData["error"] = "Apagar arquivo - Houve um erro na página. Favor tentar novamente";

                return RedirectToAction(nameof(Index));
            }

            var file = _raRouteService.GetFileByName(id);

            if (file == null || file.Error != "")
            {
                TempData["error"] = "Apagar arquivo - A API está fora do ar. Favor tentar novamente";

                return RedirectToAction(nameof(Index));
            }

            return View(file);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteFile(string id, HistoryGenerateFile hgFile)
        {
            string folder = _appEnvironment.WebRootPath + "\\File\\";
            string pathFile = folder + id;

            FileInfo file = new(pathFile);

            file.Delete();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(User user)
        {
            if (string.IsNullOrEmpty(user.Name) || string.IsNullOrWhiteSpace(user.Name))
            {
                TempData["error"] = "O nome do usuário não pode ser vazio.";

                return RedirectToAction(nameof(Index));
            }
            else if (string.IsNullOrEmpty(user.Username) || string.IsNullOrWhiteSpace(user.Username))
            {
                TempData["error"] = "A senha não pode ser vazia.";

                return RedirectToAction(nameof(Index));
            }

            var userLoginSearch = await new ConnectToUserApi().GetUserByUsername(user.Username);

            if (userLoginSearch == null || userLoginSearch.Error != "")
            {
                TempData["error"] = "Usuário - A API está fora do ar.";

                return RedirectToAction(nameof(Index));
            }

            if (userLoginSearch.Username == user.Username && userLoginSearch.Name == user.Name)
            {
                return RedirectToAction("NewPassword", userLoginSearch);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecoveryUser(User user)
        {
            if (string.IsNullOrEmpty(user.Name) || string.IsNullOrWhiteSpace(user.Name))
            {
                TempData["error"] = "O nome do usuário não pode ser vazio.";

                return RedirectToAction(nameof(Index));
            }
            else if (string.IsNullOrEmpty(user.Username) || string.IsNullOrWhiteSpace(user.Username))
            {
                TempData["error"] = "O usuário não pode ser vazio.";

                return RedirectToAction(nameof(Index));
            }

            var userLoginSearch = await new ConnectToUserApi().GetUserByUsername(user.Username);

            if (userLoginSearch == null || userLoginSearch.Error != "")
            {
                TempData["error"] = "O usuário não existe.";

                return RedirectToAction(nameof(Index));
            }

            if (userLoginSearch.Username == user.Username && userLoginSearch.Name == user.Name)
            {
                User userTemp = new() { Id = userLoginSearch.Id, Name = userLoginSearch.Name, Role = "" };
                return RedirectToAction("NewPassword", userTemp);
            }

            return View();
        }

        public IActionResult NewPassword(User userTemp)
        {
            return View(userTemp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewPassword(string id, User user)
        {
            if (string.IsNullOrEmpty(user.Password) || string.IsNullOrWhiteSpace(user.Password))
            {
                TempData["error"] = "A senha não pode ser vazia.";

                return RedirectToAction(nameof(Index));
            }

            var userSearch = await new ConnectToUserApi().GetUserById(id);

            user.Name = userSearch.Name;
            user.Username = userSearch.Username;
            user.Role = userSearch.Role;

            var userWithNewPassword = await new ConnectToUserApi().EditUser(user);

            if (userWithNewPassword == null || userWithNewPassword.Error != "")
            {
                TempData["error"] = "Usuário - A API está fora do ar.";

                return RedirectToAction(nameof(Index));
            }

            TempData["success"] = "Senha alterada com sucesso!";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditUser(string id)
        {
            ViewBag.User = HttpContext.User.Identity.Name;

            if (HttpContext.User.IsInRole("adm"))
                ViewBag.Role = "adm";
            else
                ViewBag.Role = "user";

            if (id == null)
            {
                TempData["error"] = "Usuário - Houve um erro na página. Favor tentar novamente";

                return RedirectToAction(nameof(Index));
            }

            var user = await new ConnectToUserApi().GetUserById(id);

            user.Password = "";

            if (user == null || user.Error != "")
            {
                TempData["error"] = "Usuário - A API está fora do ar. Favor tentar novamente";

                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, User user)
        {
            if (string.IsNullOrEmpty(user.Name) || string.IsNullOrWhiteSpace(user.Name))
            {
                TempData["error"] = "O nome do usuário não pode ser vazio.";

                return RedirectToAction(nameof(Index));
            }
            else if (string.IsNullOrEmpty(user.Username) || string.IsNullOrWhiteSpace(user.Username))
            {
                TempData["error"] = "O usuário não pode ser vazio.";

                return RedirectToAction(nameof(Index));
            }

            var userSearch = await new ConnectToUserApi().GetUserById(id);

            if (userSearch == null || userSearch.Error != "")
            {
                TempData["error"] = "Usuário - A API está fora do ar.";

                return RedirectToAction(nameof(Index));
            }

            if (user.Username != userSearch.Username)
            {
                var userByUsername = await new ConnectToUserApi().GetUserByUsername(user.Username);

                if (userByUsername != null)
                {
                    TempData["error"] = "Já existe um usuário cadastrado com o usuário informado.";

                    return RedirectToAction(nameof(Index));
                }
            }

            userSearch.Name = user.Name;
            userSearch.Username = user.Username;
            userSearch.Role = user.Role;

            if (!string.IsNullOrEmpty(user.Password))
                userSearch.Password = user.Password;

            var userInsertion = await new ConnectToUserApi().EditUser(userSearch);

            if (userInsertion == null || userInsertion.Error != "")
            {
                TempData["error"] = "Usuário - A API está fora do ar.";

                return RedirectToAction(nameof(Index));
            }

            TempData["success"] = "Usuário editado com sucesso!";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            ViewBag.User = HttpContext.User.Identity.Name;

            if (HttpContext.User.IsInRole("adm"))
                ViewBag.Role = "adm";
            else
                ViewBag.Role = "user";

            if (id == null)
            {
                TempData["error"] = "Usuário - Houve um erro na página. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            var user = await new ConnectToUserApi().GetUserById(id);

            if (user == null || user.Error != "")
            {
                TempData["error"] = "Usuário - A API está fora do ar. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }
            else if (user.Username == ViewBag.User)
            {
                TempData["error"] = "Você não pode excluir seu próprio usuário.";

                return RedirectToAction(nameof(Index));
            }

            user.Password = "";

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id, User user)
        {
            var userRemove = await new ConnectToUserApi().RemoveUser(id);

            if (userRemove.Error != "ok")
            {
                TempData["error"] = "Usuário - Houve um erro na exclusão do usuário. Favor tentar novamente.";

                return RedirectToAction(nameof(Index));
            }

            TempData["success"] = "Usuário apagado com sucesso!";

            return RedirectToAction(nameof(Index));
        }
    }
}
