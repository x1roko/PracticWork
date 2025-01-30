using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManagerMiac.Data;
using TaskManagerMiac.Models;
using TaskManagerMiac.Service;

namespace TaskManagerMiac.Controllers
{
    [Route("Tasks/[action]")]
    [Authorize(Roles = "admin, default, group_admin, root")]
    public class TaskBodiesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;
        private readonly DocumentsService _documentsService;

        public TaskBodiesController(AppDbContext context, AuthService authService, DocumentsService documentsService)
        {
            _context = context;
            _authService = authService;
            _documentsService = documentsService;
        }

        // GET: TaskBodies

        public async Task<IActionResult> Index()
        {
            //проверка что пользователь авторизован
            if (!_authService.IsAuthorizedUser(HttpContext.Session)) // данных пользователя нет в сессии
            {
                Console.WriteLine("TaskBodiesController: данных пользователя нет в сессии");
                return RedirectToAction("Index", "Auth");
            }
            return RedirectToAction(nameof(CreatedTasks));
        }
        public async Task<IActionResult> CreatedTasks(bool? archiveTasks)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            var user = _authService.GetUserFromSession(HttpContext.Session);
            try
            {
                var createdTasks = await _context.TaskBodies
                    .Where(t => t.IdUserCreator == user.IdUser)
                    .Include(t => t.IdPriorityNavigation)
                    .Include(t => t.IdTaskTypeNavigation)
                    .Include(t => t.IdUserCreatorNavigation)
                    .Include(t => t.TaskSteps)
                        .ThenInclude(ts => ts.IdTaskStateNavigation).ToListAsync();
                ViewBag.archiveTasks = archiveTasks;
                if (archiveTasks == true)
                    return View(createdTasks.Where(t => t.State != "Создана" && t.State != "В работе"));
                else
                    return View(createdTasks.Where(t => t.State == "Создана" || t.State == "В работе"));
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"TaskBodiesController - CreateTasks - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }
        public async Task<IActionResult> OwnedTasks()
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            var user = _authService.GetUserFromSession(HttpContext.Session);
            try
            {
                var ownedTasks = _context.TaskBodies
                    .Where(t =>
                        t.TaskSteps
                                .Any(t => (t.IdTaskStateNavigation.Title == "Создана" || t.IdTaskStateNavigation.Title == "В работе") && t.UserHasTaskSteps
                                .Any(u => u.IdUser == user.IdUser)))
                    .Include(t => t.IdPriorityNavigation)
                    .Include(t => t.IdUserCreatorNavigation);
                await _context.UserHasTaskSteps.Where(u => u.IdUser == user.IdUser).ForEachAsync(x => x.IsChecked = true);
                await _context.SaveChangesAsync();
                return View(await ownedTasks.ToListAsync());
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"TaskBodiesController - OwnedTasks - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }



        // GET: TaskBodies/Create
        public async Task<IActionResult> Create(int? id)
        {
            try
            {
                ViewData["IdPriority"] = new SelectList(_context.Priorities, "IdPriority", "Title");
                ViewData["IdTaskType"] = new SelectList(_context.TaskTypes, "IdTaskType", "Title");
                if (id != null)
                {
                    var taskBody = await _context.TaskBodies.Include(tb => tb.TaskSteps).FirstOrDefaultAsync(tb => tb.IdTask == id);

                    if (taskBody == null)
                    {
                        return RedirectToAction("CustomError", "Home", new { errorText = "Не найдена задача" });
                    }

                    if (taskBody.State != "Отклонена")
                    {
                        return RedirectToAction("CustomError", "Home", new { errorText = "задача не отклонена" });
                    }

                    if (taskBody.IdUserCreator != _authService.GetUserFromSession(HttpContext.Session)?.IdUser)
                    {
                        return RedirectToAction("CustomError", "Home", new { errorText = "Вы не создатель заявки" });
                    }
                    var newTaskBody = new TaskBody();
                    _context.Entry(newTaskBody).CurrentValues.SetValues(taskBody);
                    newTaskBody.IdTask = 0;
                    return View(newTaskBody);
                }
                return View();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"TaskBodiesController - Create - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        // POST: TaskBodies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTask,Title,Description,IdTaskType,IdPriority,DeadlineDate")] TaskBody taskBody)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            try
            {
                taskBody.IdUserCreator = _authService.GetUserFromSession(HttpContext.Session).IdUser;
                _context.Add(taskBody);
                if (taskBody.IdTaskType == 2) // тип заявки "обычный"
                {
                    await _context.SaveChangesAsync();
                    TempData["IdTask"] = taskBody.IdTask;
                    return RedirectToAction("CreateTaskSteps", "TaskBodies");
                }

                if (taskBody.IdTaskType == 4) // тип заявки "персональный"
                {
                    await _context.SaveChangesAsync();
                    TempData["IdTask"] = taskBody.IdTask;
                    return RedirectToAction("SendTaskToPerson", "TaskBodies");
                }

                TaskStep taskStep = new TaskStep();
                //var firstPath = _context.TaskTypePaths.FirstOrDefault(t => t.Index == 1 && t.IdTaskType == taskBody.IdTaskType);
                var path = await _context.TaskTypePaths.OrderBy(t => t.Index).Where(t => t.IdTaskType == taskBody.IdTaskType).Include(t => t.IdGroupNavigation).ToListAsync();
                if (path.Count() > 0)
                {
                    taskStep.GroupIdPerformer = path.FirstOrDefault().IdGroup;
                    var pathCount = path.Count;
                    for (int i = 0; i < pathCount; i++)
                    {
                        TaskBodyPath taskBodyPath = new();
                        taskBodyPath.TaskBodyIdTaskNavigation = taskBody;
                        taskBodyPath.GroupIdGroupNavigation = path[i].IdGroupNavigation;
                        taskBodyPath.Index = i + 1;
                        taskBody.TaskBodyPaths.Add(taskBodyPath);
                    }
                }
                else
                {
                    taskStep.GroupIdPerformer = 2; // 2 -  не определён (заглушка)
                }
                taskStep.IdTaskState = _context.TaskStates.FirstOrDefault(ts => ts.Title == "Создана").IdTaskState; //по умолчанию создана
                taskStep.Description = "";
                taskStep.Index = 1;
                taskStep.IdTaskNavigation = taskBody;
                taskBody.TaskSteps.Add(taskStep);
                await _context.SaveChangesAsync();
                //if (true) // добавить условие для проверки нужно ли генерировать документ
                //    return RedirectToAction("GenerateFinancialDocument", "Documents", new { id = taskBody.IdTask });
                return RedirectToAction(nameof(Details), new { id = taskBody.IdTask });
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"TaskBodiesController - Create - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");

            if (id == null)
            {
                return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
            }
            try
            {
                var tb = _context.TaskBodies
                    .Include(tb => tb.Documents.Where(d => !d.IsDeleted))
                        .ThenInclude(d => d.FinancialTaskDocumentNavigation)
                    .Include(tb => tb.IdUserCreatorNavigation)
                    .Include(tb => tb.Commentaries.OrderByDescending(c => c.IdComment))
                    .FirstOrDefault(tb => tb.IdTask == id);
                if (tb == null)
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
                }

                ViewBag.taskBody = tb;

                var taskSteps = _context.TaskSteps.Where(t => t.IdTask == id)
                    .Include(t => t.GroupIdPerformerNavigation)
                    .Include(t => t.IdTaskNavigation)
                    .Include(t => t.IdTaskStateNavigation)
                    .Include(t => t.UserHasTaskSteps)
                        .ThenInclude(t => t.IdUserNavigation);

                if (taskSteps == null)
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
                }

                return View(await taskSteps.ToListAsync());
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"TaskBodiesController - Details - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        public IActionResult CreateTaskSteps(
            int? idTask
            )
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            if (idTask != null)
            {
                TempData["IdTask"] = idTask;
            }
            ViewData["IdGroup"] = new SelectList(_context.Groups, "IdGroup", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTaskSteps(int idGroup)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            try
            {
                if (TempData["IdTask"] != null && TempData["IdTask"] is int idTask)
                {
                    var taskBody = await _context.TaskBodies.FindAsync(idTask);
                    if (taskBody == null)
                    {
                        return NotFound("taskBody не найдена");
                    }

                    var selectedGroup = await _context.Groups.FindAsync(idGroup);
                    if (selectedGroup == null)
                    {
                        return NotFound("selectedGroup не найдена");
                    }

                    TaskStep taskStep = new TaskStep()
                    {
                        IdTaskNavigation = taskBody,
                        GroupIdPerformerNavigation = selectedGroup,
                        IdTaskState = _context.TaskStates.FirstOrDefault(ts => ts.Title == "Создана").IdTaskState,
                        Description = "",
                        Index = -1
                    };
                    taskBody.TaskSteps.Add(taskStep);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = idTask });
                }

                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"TaskBodiesController - CreateTaskSteps - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        public IActionResult SendTaskToPerson(
    int? idTask
    )
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            if (idTask != null)
            {
                TempData["IdTask"] = idTask;
            }
            ViewData["IdUser"] = new SelectList(_context.Users, "IdUser", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendTaskToPerson(int idUser)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            try
            {
                if (TempData["IdTask"] != null && TempData["IdTask"] is int idTask)
                {
                    var taskBody = await _context.TaskBodies.FindAsync(idTask);
                    if (taskBody == null)
                    {
                        return NotFound("taskBody не найден");
                    }

                    var selectedUser = await _context.Users.FindAsync(idUser);
                    if (selectedUser == null)
                    {
                        return NotFound("selectedUser не найден");
                    }

                    TaskStep taskStep = new TaskStep()
                    {
                        IdTaskNavigation = taskBody,
                        GroupIdPerformer = selectedUser.IdGroup,
                        IdTaskState = _context.TaskStates.FirstOrDefault(ts => ts.Title == "Создана").IdTaskState,
                        Description = "",
                        Index = -1
                    };
                    UserHasTaskStep userHasTaskStep = new();
                    userHasTaskStep.IdUserNavigation = selectedUser;
                    userHasTaskStep.IsResponsible = true;
                    taskStep.UserHasTaskSteps.Add(userHasTaskStep);
                    taskBody.TaskSteps.Add(taskStep);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = idTask });
                }

                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"TaskBodiesController - SendTaskToPerson - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }
    }
}
