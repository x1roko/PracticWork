using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerMiac.Data;
using TaskManagerMiac.Models;
using TaskManagerMiac.Service;

namespace TaskManagerMiac.Controllers
{
    public class TaskStepsController : Controller
    {

        private readonly AppDbContext _context;
        private readonly AuthService _authService;

        public TaskStepsController(AppDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }


        public async Task<IActionResult> Complete(int? id)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            if (id == null)
            {
                return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
            }
            var user = _authService.GetUserFromSession(HttpContext.Session);
            try
            {
                var taskStep = await _context.TaskSteps
                    .Include(ts => ts.IdTaskNavigation)
                    .Include(ts => ts.UserHasTaskSteps)
                    .FirstOrDefaultAsync(ts => ts.IdTaskStep == id);
                if (taskStep == null)
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
                }

                var responsibles = taskStep.UserHasTaskSteps.Where(u => u.IsResponsible); // ответственные за заявку
                if (!responsibles.Any(r => r.IdUser == user.IdUser))
                {
                    if (!Enumerable.Range(1, 3).Contains(user.IdRole)) // Если пользователь не относится к админам
                    {
                        return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });
                    }
                }
                ViewData["TaskName"] = taskStep.IdTaskNavigation.Title;
                return View(taskStep);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"TaskStepsController - Complete - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Complete(int id, [Bind("IdTaskStep,IdTask,Description")] TaskStep taskStep)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");

            if (id != taskStep.IdTaskStep)
            {
                return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
            }
            var user = _authService.GetUserFromSession(HttpContext.Session);
            try
            {
                var taskStepFromDb = _context.TaskSteps
                    .Include(t => t.GroupIdPerformerNavigation)
                    .Include(t => t.IdTaskNavigation)
                        .ThenInclude(tb => tb.TaskBodyPaths)
                    .Include(t => t.IdTaskStateNavigation)
                    .Include(t => t.UserHasTaskSteps)
                    .FirstOrDefault(ts => ts.IdTaskStep == taskStep.IdTaskStep);
                var responsibles = taskStepFromDb.UserHasTaskSteps.Where(u => u.IsResponsible); // ответственные за заявку
                var taskBody = taskStepFromDb.IdTaskNavigation;
                if (!responsibles.Any(r => r.IdUser == user.IdUser))
                {
                    if (!Enumerable.Range(1, 3).Contains(user.IdRole)) // Если пользователь не относится к админам
                    {
                        return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });
                    }
                }
                if (taskStepFromDb.IdTaskStateNavigation.Title == "Одобрена")
                    return RedirectToAction("Details", "TaskBodies", new { id = taskStep.IdTask });
                taskStepFromDb.ChangeStateDate = DateTime.Now;
                taskStepFromDb.Description = taskStep.Description;
                if (user.IdRole == 4)
                {
                    taskStepFromDb.IdTaskState = _context.TaskStates.FirstOrDefault(ts => ts.Title == "Ожидает одобрения руководителя").IdTaskState;
                    _context.Update(taskStepFromDb);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "TaskBodies", new { id = taskStep.IdTask });
                }
                taskStepFromDb.IdTaskState = _context.TaskStates.FirstOrDefault(ts => ts.Title == "Одобрена").IdTaskState;
                var path = taskBody.TaskBodyPaths.FirstOrDefault(p => p.Index == taskStepFromDb.Index + 1);
                if (path != null)
                {
                    TaskStep taskStep2 = new TaskStep();
                    taskStep2.GroupIdPerformer = path.GroupIdGroup;
                    taskStep2.IdTaskState = _context.TaskStates.FirstOrDefault(ts => ts.Title == "Создана").IdTaskState;
                    taskStep2.Description = "";
                    taskStep2.Index = taskStepFromDb.Index + 1;
                    taskStep2.IdTaskNavigation = taskBody;
                    taskBody.TaskSteps.Add(taskStep2);
                }
                _context.Update(taskStepFromDb);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "TaskBodies", new { id = taskStep.IdTask });
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"TaskStepsController - Complete - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        public async Task<IActionResult> Deny(int? id)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");

            if (id == null)
            {
                return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
            }
            var user = _authService.GetUserFromSession(HttpContext.Session);
            try
            {
                var taskStep = await _context.TaskSteps
                    .Include(ts => ts.IdTaskNavigation)
                    .Include(ts => ts.UserHasTaskSteps)
                    .FirstOrDefaultAsync(ts => ts.IdTaskStep == id);
                if (taskStep == null)
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
                }
                var responsibles = taskStep.UserHasTaskSteps.Where(u => u.IsResponsible); // ответственные за заявку

                if (!responsibles.Any(r => r.IdUser == user.IdUser)) // Проверяем является ли пользователь частью таскстепа
                {
                    if (!Enumerable.Range(1, 3).Contains(user.IdRole)) // Если пользователь не относится к админам
                    {
                        return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });
                    }
                }

                ViewData["TaskName"] = taskStep.IdTaskNavigation.Title;
                return View(taskStep);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"TaskStepsController - Deny - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Deny(int id, [Bind("IdTaskStep,IdTask,Description")] TaskStep taskStep)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");

            if (id != taskStep.IdTaskStep)
            {
                return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
            }
            try
            {
                var user = _authService.GetUserFromSession(HttpContext.Session);
                var taskStepFromDb = _context.TaskSteps
                    .Include(t => t.GroupIdPerformerNavigation)
                    .Include(t => t.IdTaskNavigation)
                    .Include(t => t.IdTaskStateNavigation)
                    .Include(t => t.UserHasTaskSteps)
                    .FirstOrDefault(ts => ts.IdTaskStep == taskStep.IdTaskStep);

                var responsibles = taskStepFromDb.UserHasTaskSteps.Where(u => u.IsResponsible); // ответственные за заявку
                if (!responsibles.Any(r => r.IdUser == user.IdUser) && responsibles.Count() >= 1)
                {
                    if (!Enumerable.Range(1, 3).Contains(user.IdRole))
                    {
                        return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });
                    }
                }
                taskStepFromDb.ChangeStateDate = DateTime.Now;
                taskStepFromDb.Description = taskStep.Description;
                if (user.IdRole == 4)
                {
                    taskStepFromDb.IdTaskState = _context.TaskStates.FirstOrDefault(ts => ts.Title == "Ожидает отклонения руководителя").IdTaskState;
                    _context.Update(taskStepFromDb);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "TaskBodies", new { id = taskStep.IdTask });
                }
                taskStepFromDb.IdTaskState = _context.TaskStates.FirstOrDefault(ts => ts.Title == "Отклонена").IdTaskState;
                _context.Update(taskStepFromDb);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"TaskStepsController - Deny - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
            return RedirectToAction("Details", "TaskBodies", new { id = taskStep.IdTask });
        }

        public async Task<IActionResult> Reopen(int id)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            try
            {
                var taskStep = await _context.TaskSteps.FindAsync(id);

                if (taskStep == null)
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
                }

                var user = _authService.GetUserFromSession(HttpContext.Session);
                taskStep.IdTaskState = 2;
                if (!Enumerable.Range(1, 3).Contains(user.IdRole))
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });
                }
                _context.Update(taskStep);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "TaskBodies", new { id = taskStep.IdTask });
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"TaskStepsController - Reopen - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        public async Task<IActionResult> Details(int? id) // Кто может получить доступ?...
        {
            var user = _authService.GetUserFromSession(HttpContext.Session);
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
                await Console.Out.WriteLineAsync($"TaskStepsController - Details - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }
    }
}
