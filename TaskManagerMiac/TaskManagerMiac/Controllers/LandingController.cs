using Microsoft.AspNetCore.Mvc;

namespace TaskManagerMiac.Controllers
{
    public class LandingController : Controller
    {
        public IActionResult Index(string? code, string? state)
        {
            if (code != null & state != null) // По умолчанию госулуги перенаправляют сюда
            {
                return RedirectToAction("Login", "Auth", new { code, state });
            }
            return View();
        }
    }
}
