using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerMiac.Data;
using TaskManagerMiac.Models;

namespace TaskManagerMiac.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotifyController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotifyController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifyCount(string userSnils)
        {
            if (string.IsNullOrEmpty(userSnils)) 
            { 
                return BadRequest("userSnils is empty"); 
            }

            var user = await _context.Users.FirstOrDefaultAsync(u=>u.Snils == userSnils);
            if (user == null)
            {
                return NotFound(); 
            }

            var ownedTasks = _context.TaskBodies.AsNoTracking()
                .Where(t =>
                    t.TaskSteps
                            .Any(t => (t.IdTaskStateNavigation.Title == "Создана" || t.IdTaskStateNavigation.Title == "В работе") && t.UserHasTaskSteps
                            .Any(u => u.IdUser == user.IdUser)))
                .Include(t => t.IdPriorityNavigation)
                .Include(t => t.IdUserCreatorNavigation);
            var res = ownedTasks.Count(o => o.TaskSteps.Any(ts => ts.UserHasTaskSteps.Any(u => u.IdUser == user.IdUser && !u.IsChecked)));

            return Ok(res);
        }
    }
}
