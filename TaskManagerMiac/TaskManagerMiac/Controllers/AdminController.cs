using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManagerMiac.Data;
using TaskManagerMiac.Models;
using TaskManagerMiac.Service;

namespace TaskManagerMiac.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;
        private readonly List<int> _permittedRoles = new List<int> { 1, 2, 3 };

        public AdminController(AppDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [Authorize(Roles = "admin, group_admin, root")]
        public async Task<IActionResult> Index()
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            var user = _authService.GetUserFromSession(HttpContext.Session);
            if (!_permittedRoles.Contains(user.IdRole))
                return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });
            return View();
        }

        [Authorize(Roles = "admin, group_admin, root")]
        public async Task<IActionResult> GroupTasks(bool? archiveTasks)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            var user = _authService.GetUserFromSession(HttpContext.Session);
            if (!_permittedRoles.Contains(user.IdRole))
                return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });
            try
            {
                var taskBodies = await _context.TaskBodies
                    .Where(t => t.TaskSteps
                        .Any(ts => ts.GroupIdPerformer == user.IdGroup))
                    .OrderByDescending(t => t.CreationDate)
                    .Include(t => t.IdPriorityNavigation)
                    .Include(t => t.IdUserCreatorNavigation)
                    .Include(t => t.TaskSteps)
                        .ThenInclude(ts => ts.UserHasTaskSteps)
                        .ThenInclude(u => u.IdUserNavigation)
                    .ToListAsync();

                ViewBag.groupId = user.IdGroup;
                ViewBag.archiveTasks = archiveTasks;

                if (archiveTasks == true)
                    return View(taskBodies.Where(t => t.State != "Создана" && t.State != "В работе"));
                else
                    return View(taskBodies.Where(t => t.State == "Создана" || t.State == "В работе"));
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"AdminController - GroupTasks - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        [Authorize(Roles = "root, admin")]
        public async Task<IActionResult> AllTasks(bool? archiveTasks)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            var user = _authService.GetUserFromSession(HttpContext.Session);
            if (!_permittedRoles.Contains(user.IdRole))
                return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });
            try
            {
                var taskBodies = await _context.TaskBodies
                    .OrderByDescending(t => t.CreationDate)
                    .Include(t => t.IdPriorityNavigation)
                    .Include(t => t.IdUserCreatorNavigation)
                    .Include(t => t.TaskSteps)
                        .ThenInclude(ts => ts.UserHasTaskSteps)
                            .ThenInclude(u => u.IdUserNavigation)
                    .Include(t => t.TaskSteps)
                        .ThenInclude(ts => ts.GroupIdPerformerNavigation)
                    .Include(t => t.TaskSteps)
                        .ThenInclude(ts => ts.IdTaskStateNavigation)
                    .ToListAsync();

                ViewBag.groupId = user.IdGroup;
                ViewBag.archiveTasks = archiveTasks;

                if (archiveTasks == true)
                    return View(taskBodies.Where(t => t.State != "Создана" && t.State != "В работе"));
                else
                    return View(taskBodies.Where(t => t.State == "Создана" || t.State == "В работе"));
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"AdminController - AllTasks - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        [Authorize(Roles = "admin, group_admin, root")]
        public async Task<IActionResult> ChangePerformer(int? id)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");

            if (id == null)
            {
                return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
            }
            try
            {
                var taskStep = await _context.TaskSteps
                    .Include(ts => ts.IdTaskNavigation)
                    .FirstOrDefaultAsync(ts => ts.IdTaskStep == id);
                if (taskStep == null)
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
                }
                ViewData["TaskName"] = taskStep.IdTaskNavigation.Title;
                var users = _context.Users.Where(u => !u.UserHasTaskSteps.Any(u => u.IdTaskStep == id));
                if (taskStep.IdTaskNavigation.IdTaskType != 2)
                    users = users.Where(u => u.IdGroup == taskStep.GroupIdPerformer);
                ViewData["UserIdPerformer"] = new SelectList(users, "IdUser", "FullName", taskStep.UserHasTaskSteps);
                UserHasTaskStep userHasTaskStep = new()
                {
                    IdTaskStep = taskStep.IdTaskStep
                };
                return View(userHasTaskStep);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"AdminController - ChangePerformer - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        [Authorize(Roles = "admin, group_admin, root")]
        [HttpPost]
        public async Task<IActionResult> ChangePerformer(int id, [Bind("IdTaskStep, IdUser, IsResponsible")] UserHasTaskStep userHasTaskStep)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            var user = _authService.GetUserFromSession(HttpContext.Session);
            if (!_permittedRoles.Contains(user.IdRole))
                return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });
            try
            {
                if (id != userHasTaskStep.IdTaskStep)
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
                }
                var taskStep = await _context.TaskSteps.FindAsync(userHasTaskStep.IdTaskStep);
                taskStep.UserHasTaskSteps.Add(userHasTaskStep);
                taskStep.IdTaskState = _context.TaskStates.FirstOrDefault(ts => ts.Title == "В работе").IdTaskState;
                try
                {
                    _context.Update(taskStep);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
                }
                return RedirectToAction("ManagePerformers", new { id });
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"AdminController - ChangePerformer - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        [Authorize(Roles = "admin, group_admin, root")]
        public async Task<IActionResult> ManagePerformers(int? id)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");

            if (id == null)
            {
                return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
            }
            try
            {
                var taskStep = await _context.TaskSteps
                    .Include(t => t.GroupIdPerformerNavigation)
                    .Include(t => t.IdTaskNavigation)
                    .Include(t => t.IdTaskStateNavigation)
                    .Include(t => t.UserHasTaskSteps)
                        .ThenInclude(u => u.IdUserNavigation)
                    .FirstOrDefaultAsync(ts => ts.IdTaskStep == id);
                if (taskStep == null)
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
                }
                ViewBag.TaskBody = taskStep.IdTaskNavigation;
                return View(taskStep);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"AdminController - ManagePerformers - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        [Authorize(Roles = "admin, group_admin, root")]
        public async Task<IActionResult> DeletePerformer(int idTaskStep, int idUser)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            var user = _authService.GetUserFromSession(HttpContext.Session);
            if (!_permittedRoles.Contains(user.IdRole))
                return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });
            try
            {
                var taskStep = await _context.TaskSteps
                    .Include(ts => ts.UserHasTaskSteps)
                    .FirstOrDefaultAsync(ts => ts.IdTaskStep == idTaskStep);
                if (taskStep == null)
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
                }
                var userForDelete = taskStep.UserHasTaskSteps.FirstOrDefault(u => u.IdUser == idUser);
                if (userForDelete == null)
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
                }
                try
                {
                    taskStep.UserHasTaskSteps.Remove(userForDelete);
                    _context.Update(taskStep);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
                }
                return RedirectToAction("ManagePerformers", new { id = idTaskStep });
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"AdminController - DeletePerformer - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        [Authorize(Roles = "admin, group_admin, root")]
        public async Task<IActionResult> GroupWaitingTasks()
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            var user = _authService.GetUserFromSession(HttpContext.Session);
            if (!_permittedRoles.Contains(user.IdRole))
                return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });
            try
            {
                var taskBodies = await _context.TaskBodies
                    .Where(t => t.TaskSteps
                        .Any(ts => ts.GroupIdPerformer == user.IdGroup))
                    .OrderByDescending(t => t.CreationDate)
                    .Include(t => t.IdPriorityNavigation)
                    .Include(t => t.IdUserCreatorNavigation)
                    .Include(t => t.TaskSteps)
                        .ThenInclude(ts => ts.UserHasTaskSteps)
                        .ThenInclude(u => u.IdUserNavigation)
                        .Where(tb => tb.TaskSteps.Any(ts => ts.IdTaskState == 5 || ts.IdTaskState == 6))
                    .ToListAsync();

                ViewBag.groupId = user.IdGroup;
                return View(taskBodies);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"AdminController - GroupWaitingTasks - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }
    }
}
