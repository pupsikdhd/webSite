using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using tutorialAsp.NetCore.Data.Context;
using tutorialAsp.NetCore.Data.Models.ViewModel;

namespace AuthApp.Controllers
{
    public class AccountController : Controller
    {
        public string hash(string text)
        {
            string output; 
            MD5 MD5Hash = MD5.Create(); 
            byte[] inputBytes = Encoding.ASCII.GetBytes(text); 
            byte[] hash = MD5Hash.ComputeHash(inputBytes);
            output = Convert.ToHexString(hash); 
            return output;
        }


        private DBContext db;
        public AccountController(DBContext context)
        {
            db = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Username == model.UserName && u.HashPassword == hash(model.Password));
                if (user != null)
                {
                    if (statusMessages.ContainsKey(user.Status))
                    {
                        if (user.Status == "active")
                        {
                            await Authenticate(model.UserName,user.Role);
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", statusMessages[user.Status]);
                            return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Неизвестный статус аккаунта");
                        return View(model);
                    }
                }

                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        Dictionary<string, string> statusMessages = new Dictionary<string, string>
            {
                { "ban", "аккаунт заблокирован" },
                { "active", "аккаунт активен" },
                { "deleted", "аккаунт удалён" }
            };



        public IActionResult AccessDenied()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Username == model.UserName);
                if (user == null)
                {
                    var newUser = new User
                    {
                        Username = model.UserName,
                        HashPassword = hash(model.Password),
                        Status = "active",
                        Role = "user"
                    };
                    db.Users.Add(newUser);
                    await db.SaveChangesAsync();

                    await Authenticate(model.UserName, newUser.Role);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                }
            }
            return View(model);
        }

        private async Task Authenticate(string userName, string roles)
        {
            var roleList = roles.Split(',');
            var claims = new List<Claim>
    {
        new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
    };

            foreach (var role in roleList)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
            }

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }



        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}