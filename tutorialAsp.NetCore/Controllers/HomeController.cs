using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using tutorialAsp.NetCore.Models;

namespace tutorialAsp.NetCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin , tester")]
        public async Task<IActionResult> Command()
        {
                return View();
        }

        [Authorize(Roles = "admin , tester")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Command(string command)
        {

            if (string.IsNullOrWhiteSpace(command))
            {
                ModelState.AddModelError("", "Команда не может быть пустой");
                return View();
            }

            try
            {
                var processInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe", $"/c {command}")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = System.Diagnostics.Process.Start(processInfo))
                {
                    using (var reader = process.StandardOutput)
                    {
                        string result = await reader.ReadToEndAsync();
                        ViewBag.CommandOutput = result;
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Во время выполнения команды произошла ошибка: {ex.Message}");
                return View();
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
