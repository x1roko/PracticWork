using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerMiac.Data;
using TaskManagerMiac.Models;
using TaskManagerMiac.Service;

namespace TaskManagerMiac.Controllers
{
    public class TaskBodyCommentariesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;

        public TaskBodyCommentariesController(AppDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(int id, string? text)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            var user = _authService.GetUserFromSession(HttpContext.Session);
            try
            {
                var taskBody = await _context.TaskBodies
                    .Include(tb => tb.TaskSteps)
                        .ThenInclude(ts => ts.UserHasTaskSteps)
                    .FirstOrDefaultAsync(t => t.IdTask == id);
                if (taskBody == null)
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
                }

                if (!(taskBody.TaskSteps.Any(ts => ts.UserHasTaskSteps.Any(u => u.IdUser == user.IdUser)) || taskBody.IdUserCreator == user.IdUser))
                    return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });

                if (text == null)
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "Комментарий не может быть пустым" });
                }
                var commentary = new TaskBodyComment();
                commentary.IdUserNavigation = user;
                commentary.Text = text;
                taskBody.Commentaries.Add(commentary);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "TaskBodies", new { id });

            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"TaskBodyCommentariesController - CreateAsync - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }
    }
}
