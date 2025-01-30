using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManagerMiac.Data;
using TaskManagerMiac.Models;

namespace TaskManagerMiac.Controllers
{
   [Authorize(Roles = "root")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = _context.Users.Include(u => u.IdGroupNavigation).Include(u => u.IdRoleNavigation);
            ViewData["IdGroup"] = new SelectList(_context.Groups, "IdGroup", "Title");

            return View(await users.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(
            [Bind("users")]
             IEnumerable<User> usersView
            )
        {
            var users = _context.Users.Include(u => u.IdGroupNavigation).Include(u => u.IdRoleNavigation);
            ViewData["IdGroup"] = new SelectList(_context.Groups, "IdGroup", "Title");

            return View(await users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
            }

            var user = await _context.Users
                .Include(u => u.IdGroupNavigation)
                .Include(u => u.IdRoleNavigation)
                .FirstOrDefaultAsync(m => m.IdUser == id);
            if (user == null)
            {
                return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
            }

            return View(user);
        }

        public IActionResult Create()
        {
            ViewData["IdGroup"] = new SelectList(_context.Groups, "IdGroup", "Title");
            ViewData["IdRole"] = new SelectList(_context.Roles, "IdRole", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUser,Snils,IdGroup,Firstname,Surname,Patronymic,IdRole,RegistrationDate,IsActive,LastEnterDate")] User user)
        {
            try
            {
                user.IsActive = true;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewData["IdGroup"] = new SelectList(_context.Groups, "IdGroup", "Title", user.IdGroup);
                ViewData["IdRole"] = new SelectList(_context.Roles, "IdRole", "Title", user.IdRole);
                return View(user);
            }
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
            }
            ViewData["IdGroup"] = new SelectList(_context.Groups, "IdGroup", "Title", user.IdGroup);
            ViewData["IdRole"] = new SelectList(_context.Roles, "IdRole", "Title", user.IdRole);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdUser,Snils,IdGroup,Firstname,Surname,Patronymic,IdRole,RegistrationDate,IsActive")] User user)
        {
            if (id != user.IdUser)
            {
                return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
            }


            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.IdUser))
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
            }

            var user = await _context.Users
                .Include(u => u.IdGroupNavigation)
                .Include(u => u.IdRoleNavigation)
                .FirstOrDefaultAsync(m => m.IdUser == id);
            if (user == null)
            {
                return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                Console.WriteLine("Пользователь не удалён, есть не выполненные задачи");
                return RedirectToAction("CustomError", "Home", new { errorText = "Не удалось удалить пользователя, есть не выполненные задачи" });
            }
      
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MakeInactive(int? id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsActive = false;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.IdUser == id);
        }
    }
}
