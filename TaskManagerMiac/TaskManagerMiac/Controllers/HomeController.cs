using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TaskManagerMiac.Models;
using TaskManagerMiac.Service;
using TaskManagerMiac.ViewModels;

namespace TaskManagerMiac.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AuthService _authService;
        private readonly MIACAuthService _miacAuthService;

        public HomeController(ILogger<HomeController> logger, AuthService authService, MIACAuthService miacAuthService)
        {
            _logger = logger;
            _authService = authService;
            _miacAuthService = miacAuthService;
        }

        public IActionResult Index(string? code, string? state)
        {
            if (code != null & state != null) // По умолчанию госулуги перенаправляют сюда
            {
                return RedirectToAction("Login", "Auth",new {code, state});
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult CustomError(string errorText)
        {
            return View(new CustomErrorViewModel { ErrorText = errorText });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
