using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerMiac.Data;
using TaskManagerMiac.Service;

namespace TaskManagerMiac.Controllers
{
    [Authorize(Roles = "admin, default, group_admin, root")]
    public class CurrentUserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;

        public CurrentUserController(AuthService authService, AppDbContext context)
        {
            _context = context;
            _authService = authService;
        }
        public async Task<IActionResult> Index()
        {
            var currentUser = _authService.GetUserFromSession(HttpContext.Session);
            if (currentUser == null)
            {
                Console.WriteLine("CurrentUserController: currentUser is null");
                return RedirectToAction("Privacy", "Home");
            }
            var userDb= await _context.Users
                .Include(u=>u.IdRoleNavigation)
                .Include(u=>u.IdGroupNavigation)
                .Where(u=>u.Snils == currentUser.Snils)
                .FirstOrDefaultAsync();
            if (userDb == null)
            {
                Console.WriteLine("CurrentUserController: userDb is null");
                return RedirectToAction("Privacy", "Home");
            }

            return View(userDb);
        }
    }
}
