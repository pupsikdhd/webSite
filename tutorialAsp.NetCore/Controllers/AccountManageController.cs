using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tutorialAsp.NetCore.Data.Context;
using tutorialAsp.NetCore.Data.Models.ViewModel.AccountManage;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace tutorialAsp.NetCore.Controllers
{
    public class AccountManageController : Controller
    {
        private readonly DBContext _dbContext;
        private readonly PasswordHasher<User> _passwordHasher;

        public AccountManageController(DBContext dbContext)
        {
            _dbContext = dbContext;
            _passwordHasher = new PasswordHasher<User>();
        }

        [Authorize]
        public IActionResult Index()
        {
            return RedirectToAction("ChangePassword");
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.HashPassword, model.Password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Неверный текущий пароль.");
                return View(model);
            }

            user.HashPassword = _passwordHasher.HashPassword(user, model.NewPassword);
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
