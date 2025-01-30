using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerMiac.Data;
using TaskManagerMiac.Models;
using TaskManagerMiac.Service;

namespace TaskManagerMiac.Controllers
{
    /// <summary>
    /// Контроллер для работы с документами, их загрузкой на сервер и отправкой пользователю
    /// </summary>
    [Authorize(Roles = "admin, default, group_admin, root")]
    public class DocumentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;
        private readonly DocumentsService _documentsService;
        private readonly IWebHostEnvironment _environment;

        public DocumentsController(AppDbContext context, AuthService authService, DocumentsService documentsService, IWebHostEnvironment environment)
        {
            _context = context;
            _authService = authService;
            _documentsService = documentsService;
            _environment = environment;
        }
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Метод для прикрепления документа к заявке
        /// </summary>
        /// <param name="id">id заявки</param>
        /// <returns></returns>
        public async Task<IActionResult> UploadDocument(int? id)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            var user = _authService.GetUserFromSession(HttpContext.Session);

            if (id == null)
            {
                return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
            }
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
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });
                }
                ViewData["TaskTitle"] = taskBody.Title;

                return View();

            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"DocumentsController - UploadDocument - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        /// <summary>
        /// Метод контроллера для загрузки документа на сервер и последующего прикрепления к заявке
        /// </summary>
        /// <param name="id">id заявки</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadDocument(int id)
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
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });
                }
                IFormFileCollection files = HttpContext.Request.Form.Files;
                var uploadPath = $"{Directory.GetCurrentDirectory()}/uploads";
                // создаем папку для хранения файлов
                Directory.CreateDirectory(uploadPath);
                foreach (var file in files)
                {
                    var doc = await _documentsService.LoadFile(taskBody.IdTask, file);
                }
                return RedirectToAction("Details", "TaskBodies", new { id });
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"DocumentsController - UploadDocument - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        /// <summary>
        /// Метод для скачивания документа с сервера
        /// </summary>
        /// <param name="id">id документа </param>
        /// <returns></returns>
        public async Task<IActionResult> DownloadDocument(int? id)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            var user = _authService.GetUserFromSession(HttpContext.Session);
            try
            {
                var document = await _context.Documents
                    .Include(d => d.TaskBodyIdTaskNavigation)
                        .ThenInclude(tb => tb.TaskSteps)
                            .ThenInclude(ts => ts.UserHasTaskSteps)
                    .FirstOrDefaultAsync(d => d.IdDocument == id);
                if (document == null || document.IsDeleted)
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
                }
                var taskBody = document.TaskBodyIdTaskNavigation;
                if (taskBody == null)
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
                }
                if (!(taskBody.TaskSteps.Any(ts => ts.UserHasTaskSteps.Any(u => u.IdUser == user.IdUser)) || taskBody.IdUserCreator == user.IdUser))
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });
                }
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", $"{ document.IdDocument}{document.Extension}");
                var file = new FileInfo(filePath);
                if (!file.Exists)
                    return NotFound(filePath);
                var stream = file.OpenRead(); // Считывание файла
                return File(stream, "application/octet-stream", $"{document.Title}{document.Extension}");
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"DocumentsController - DownloadDocument - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        /// <summary>
        /// Метод для генерации финансового документа
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> GenerateFinancialDocument(int? id)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            var user = _authService.GetUserFromSession(HttpContext.Session);

            if (id == null)
            {
                return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
            }
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
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });
                }
                return View();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"DocumentsController - GenerateFinancialDocument - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateFinancialDocument(int id, [Bind("Product,OKPD,Price,Amount,DeliverPlace,DeliverDate,Guarantee,Notes")] FinancialTaskDocument document)
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
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });
                }
                await _documentsService.GeneratePDFToTaskAsync(id, document);
                return RedirectToAction("Details", "TaskBodies", new { id });
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"DocumentsController - GenerateFinancialDocument - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        /// <summary>
        /// Метод для редактирования финансового документа
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> EditFinancialDocument(int? id)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            var user = _authService.GetUserFromSession(HttpContext.Session);

            if (id == null)
            {
                return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
            }
            try
            {
                var document = await _context.Documents
                    .Include(d => d.FinancialTaskDocumentNavigation)
                    .Include(d => d.TaskBodyIdTaskNavigation)
                        .ThenInclude(d => d.TaskSteps)
                            .ThenInclude(ts => ts.UserHasTaskSteps)
                    .FirstOrDefaultAsync(t => t.IdDocument == id);
                if (document == null)
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
                }
                var taskBody = document.TaskBodyIdTaskNavigation;
                if (!(taskBody.TaskSteps.Any(ts => ts.UserHasTaskSteps.Any(u => u.IdUser == user.IdUser)) || taskBody.IdUserCreator == user.IdUser))
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });
                }
                return View(document.FinancialTaskDocumentNavigation);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"DocumentsController - EditFinancialDocument - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFinancialDocument(int id, [Bind("IdDocument,Product,OKPD,Price,Amount,DeliverPlace,DeliverDate,Guarantee,Notes")] FinancialTaskDocument financialDocument)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            var user = _authService.GetUserFromSession(HttpContext.Session);
            try
            {
                var document = await _context.Documents
                    .Include(d => d.TaskBodyIdTaskNavigation)
                        .ThenInclude(d => d.TaskSteps)
                    .FirstOrDefaultAsync(t => t.IdDocument == id);
                if (document == null)
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
                }
                var taskBody = document.TaskBodyIdTaskNavigation;
                if (!(taskBody.TaskSteps.Any(ts => ts.UserHasTaskSteps.Any(u => u.IdUser == user.IdUser)) || taskBody.IdUserCreator == user.IdUser))
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });
                }
                await _documentsService.UpdatePDFToTaskAsync(id, financialDocument, taskBody.IdTask);
                return RedirectToAction("Details", "TaskBodies", new { id = taskBody.IdTask });
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"DocumentsController - EditFinancialDocument - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        /// <summary>
        /// Метод для согласования финансового документа
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ApproveFinancialDocument(int? id)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            var user = _authService.GetUserFromSession(HttpContext.Session);

            if (id == null)
            {
                return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
            }
            try
            {
                var document = await _context.Documents
                    .Include(d => d.FinancialTaskDocumentNavigation)
                    .Include(d => d.TaskBodyIdTaskNavigation)
                        .ThenInclude(d => d.TaskSteps)
                            .ThenInclude(ts => ts.UserHasTaskSteps)
                    .FirstOrDefaultAsync(t => t.IdDocument == id);
                if (document == null)
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
                }
                var taskBody = document.TaskBodyIdTaskNavigation;
                if (!(taskBody.TaskSteps.Any(ts => ts.UserHasTaskSteps.Any(u => u.IdUser == user.IdUser)) || taskBody.IdUserCreator == user.IdUser))
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });
                }

                return View(document.FinancialTaskDocumentNavigation);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"DocumentsController - ApproveFinancialDocument - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveFinancialDocument(int id, [Bind("IdDocument,Product,OKPD,Price,Amount,DeliverPlace,DeliverDate,Guarantee,Notes,KVR,KBK,Law,FinanceSource")] FinancialTaskDocument financialDocument)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            var user = _authService.GetUserFromSession(HttpContext.Session);
            try
            {
                var document = await _context.Documents
                    .Include(d => d.TaskBodyIdTaskNavigation)
                        .ThenInclude(d => d.TaskSteps)
                            .ThenInclude(ts => ts.UserHasTaskSteps)
                    .FirstOrDefaultAsync(t => t.IdDocument == id);
                if (document == null)
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
                }
                var taskBody = document.TaskBodyIdTaskNavigation;
                if (!(taskBody.TaskSteps.Any(ts => ts.UserHasTaskSteps.Any(u => u.IdUser == user.IdUser)) || taskBody.IdUserCreator == user.IdUser))
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });
                }
                await _documentsService.UpdatePDFToTaskAsync(id, financialDocument, taskBody.IdTask);
                return RedirectToAction("Details", "TaskBodies", new { id = taskBody.IdTask });
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"DocumentsController - ApproveFinancialDocument - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        /// <summary>
        /// Метод для удаления документа
        /// </summary>
        /// <param name="id">id документа</param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            var user = _authService.GetUserFromSession(HttpContext.Session);

            if (id == null)
            {
                return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
            }

            try
            {
                var document = await _context.Documents
                    .Include(d => d.TaskBodyIdTaskNavigation)
                        .ThenInclude(tb => tb.TaskSteps)
                            .ThenInclude(ts => ts.UserHasTaskSteps)
                    .FirstOrDefaultAsync(d => d.IdDocument == id);

                if (document == null || document.IsDeleted)
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
                }

                var taskBody = document.TaskBodyIdTaskNavigation;

                if (!(taskBody.TaskSteps.Any(ts => ts.UserHasTaskSteps.Any(u => u.IdUser == user.IdUser)) || taskBody.IdUserCreator == user.IdUser))
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });
                }
                document.IsDeleted = true;
                _context.Update(document);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "TaskBodies", new { id = document.TaskBodyIdTask });
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"DocumentsController - Delete - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }

        public async Task<IActionResult> SendFinancialDocumentToDirector(int? id)
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                return RedirectToAction("Index", "Auth");
            var user = _authService.GetUserFromSession(HttpContext.Session);

            if (id == null)
            {
                return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
            }
            try
            {
                var document = await _context.Documents
                    .Include(d => d.FinancialTaskDocumentNavigation)
                    .Include(d => d.TaskBodyIdTaskNavigation)
                        .ThenInclude(d => d.TaskSteps)
                            .ThenInclude(ts => ts.UserHasTaskSteps)
                    .FirstOrDefaultAsync(t => t.IdDocument == id);
                if (document == null)
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
                }
                var taskBody = document.TaskBodyIdTaskNavigation;
                if (!(taskBody.TaskSteps.Any(ts => ts.UserHasTaskSteps.Any(u => u.IdUser == user.IdUser)) || taskBody.IdUserCreator == user.IdUser))
                {
                    return RedirectToAction("CustomError", "Home", new { errorText = "У вас недостаточно прав" });
                }
                TaskStep taskStep = new TaskStep()
                {
                    IdTaskNavigation = taskBody,
                    GroupIdPerformer = 6, // id отдела директора = 6
                    IdTaskState = _context.TaskStates.FirstOrDefault(ts => ts.Title == "Создана").IdTaskState,
                    Description = "",
                    Index = -1
                };
                var director = await _context.Users.FirstOrDefaultAsync(u => u.IdGroup == 6 && u.IdRole == 2); // 6 - Директор, 2 - Admin, заменить на ENUM
                UserHasTaskStep userHasTaskStep = new();
                userHasTaskStep.IdUserNavigation = director;
                userHasTaskStep.IsResponsible = true;
                taskStep.UserHasTaskSteps.Add(userHasTaskStep);
                taskBody.TaskSteps.Add(taskStep);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "TaskBodies", new { id = document.TaskBodyIdTask });
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"DocumentsController - SendFinancialDocumentToDirector - {ex.Message}");
                return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
            }
        }
    }
}
