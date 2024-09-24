using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using tutorialAsp.NetCore.Data.Context;
using tutorialAsp.NetCore.Data.Models.ViewModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AuthApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly DBContext _dbContext;
        private readonly PasswordHasher<User> _passwordHasher;

        // Сообщения о статусе пользователя
        private readonly Dictionary<string, string> _statusMessages = new Dictionary<string, string>
        {
            { "ban", "аккаунт заблокирован" },
            { "active", "аккаунт активен" },
            { "deleted", "аккаунт удалён" }
        };

        public AccountController(DBContext context)
        {
            _dbContext = context;
            _passwordHasher = new PasswordHasher<User>();
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
                var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Username == model.UserName);

                if (user != null && VerifyPassword(model.Password, user.HashPassword))
                {
                    if (_statusMessages.ContainsKey(user.Status))
                    {
                        if (user.Status == "active")
                        {
                            await Authenticate(model.UserName, user.Role);
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", _statusMessages[user.Status]);
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

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var verificationResult = _passwordHasher.VerifyHashedPassword(new User(), hashedPassword, password);
            return verificationResult == PasswordVerificationResult.Success;
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
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == model.UserName);
                if (user == null)
                {
                    var newUser = new User
                    {
                        Username = model.UserName,
                        HashPassword = _passwordHasher.HashPassword(new User(), model.Password),
                        Status = "active",
                        Role = "user"
                    };

                    _dbContext.Users.Add(newUser);
                    await _dbContext.SaveChangesAsync();

                    await Authenticate(model.UserName, newUser.Role);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь с таким именем уже существует");
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

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
