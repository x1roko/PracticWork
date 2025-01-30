using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerMiac.Data;
using TaskManagerMiac.Service;

namespace TaskManagerMiac.Controllers
{
   [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;

        public DashboardController(AppDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }
        public async Task<IActionResult> Index()
        {
            //проверка что пользователь авторизован
            if (!_authService.IsAuthorizedUser(HttpContext.Session)) // данных пользователя нет в сессии
            {
                Console.WriteLine("DashboardController: данных пользователя нет в сессии");
                return RedirectToAction("Index", "Landing");
            }

            var user = _authService.GetUserFromSession(HttpContext.Session);
            try
            {
                var createdTasks = await _context.TaskBodies
                    .Where(t => t.IdUserCreator == user.IdUser)
                    .Include(t => t.IdPriorityNavigation)
                    .Include(t => t.IdTaskTypeNavigation)
                    .Include(t => t.IdUserCreatorNavigation)
                    .Include(t => t.TaskSteps)
                        .ThenInclude(ts => ts.IdTaskStateNavigation)
                        .ToListAsync();

                ViewData["CreatedTasks"] = createdTasks.Where(t => t.State == "Создана" || t.State == "В работе");

                var ownedTasks = _context.TaskBodies
                    .Where(t =>
                        t.TaskSteps
                                .Any(t => (t.IdTaskStateNavigation.Title == "Создана" || t.IdTaskStateNavigation.Title == "В работе") && t.UserHasTaskSteps
                                .Any(u => u.IdUser == user.IdUser)))
                    .Include(t => t.IdPriorityNavigation)
                    .Include(t => t.IdUserCreatorNavigation);
                ViewData["OwnedTasks"] = await ownedTasks.ToListAsync();
                ViewData["NewTasks"] = ownedTasks.Count(o => o.TaskSteps.Any(ts => ts.UserHasTaskSteps.Any(u => u.IdUser == user.IdUser && !u.IsChecked)));

                //GroupWaitingTasks
                var taskBodies = await _context.TaskBodies
                 .Where(t => t.TaskSteps
                     .Any(ts => ts.GroupIdPerformer == user.IdGroup))
                 .OrderBy(t => t.TaskSteps.OrderBy(ts => ts.Index).LastOrDefault().IdTaskState)
                     .ThenByDescending(t => t.IdPriorityNavigation.Weight)
                 .Include(t => t.IdPriorityNavigation)
                 .Include(t => t.IdUserCreatorNavigation)
                 .Include(t => t.TaskSteps)
                     .ThenInclude(ts => ts.UserHasTaskSteps)
                     .ThenInclude(u => u.IdUserNavigation)
                     .Where(tb => tb.TaskSteps.Any(ts => ts.IdTaskState == 5 || ts.IdTaskState == 6))
                 .ToListAsync();

                ViewBag.groupId = user.IdGroup;

                ViewData["GroupWaitingTasks"] = taskBodies;
                return View();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"DashboardController - Index - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        public async Task<IActionResult> GroupWaitingTasksPartial()
        {
            return PartialView();
        }
    }
}
