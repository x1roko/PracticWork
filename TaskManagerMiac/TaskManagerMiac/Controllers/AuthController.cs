using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagerMiac.Data;
using TaskManagerMiac.Dto;
using TaskManagerMiac.Extensioin;
using TaskManagerMiac.Models;
using TaskManagerMiac.Service;

namespace TaskManagerMiac.Controllers
{
    public class AuthController : Controller
    {
        private readonly MIACAuthService _miacAuthService;
        private readonly AuthService _authService;
        private readonly AppDbContext _context;

        public AuthController(MIACAuthService miacAuthService, AuthService authService, AppDbContext context)
        {
            _miacAuthService = miacAuthService;
            _authService = authService;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            if (!_authService.IsAuthorizedUser(HttpContext.Session))
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return View();
        }

        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto register)
        {
            try
            {
                var user = await _context.Users
                                .Include(user => user.IdRoleNavigation)
                                .FirstOrDefaultAsync(user => user.Snils == register.Email);

                if (user != null)
                {
                    return Unauthorized("user exists");
                }

                var newUser = await _context.Users.AddAsync(new Models.User
                {
                    Snils = register.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
                    Firstname = register.Name,
                    Surname = " ",
                    IdRole = 4,
                    IsActive = true,
                    IdGroup = 1
                });
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
                throw;
            }


        }

        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var user = await _context.Users
                .Include(user => user.IdRoleNavigation)
                .Include(user => user.IdGroupNavigation)
                .FirstOrDefaultAsync(user => user.Snils == login.Email);

            if (user == null)
            {
                return Unauthorized("Неверный логин или пароль");
            }

            if (!BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
            {
                return Unauthorized("Неверный логин или пароль");
            }


            var claims = new List<Claim>()
                {
                   new Claim("id", user.IdUser.ToString()),
                    new Claim("group", user.IdGroupNavigation.Title.ToString()),
                    new Claim(ClaimsIdentity.DefaultIssuer, user.Firstname),
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Snils),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.IdRoleNavigation.Title)
                };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(claimsPrincipal);

            SetCurrentUserToSession(user);

            return RedirectToAction("Index", "Dashboard");
        }

        // old login
        //public async Task<IActionResult> Login(string? url, string? code, string? state)
        //{
        //    //http://localhost:5031/GetDefaultUser
        //    //http://localhost:5031/GetAdminGroupUser
        //    //http://localhost:5031/GetAdminUser
        //    //http://localhost:5031/GetRootUser
        //    //http://localhost:5031/api/Users/5
        //    //http://10.1.104.100:8000
        //    // app_name = "tasker_test"
        //    // app_name = "tasker_test", code = "данные кода", state = "данные state"
        //    // tasker_test в Header, localhost:10010
        //    try
        //    {
        //        AuthUserDto? authUser = null;
        //        if (code != null && state != null)
        //        {
        //            authUser = await _miacAuthService
        //                .AuthorizeUserFromEsiaApi(code, state); // по умолчанию админ
        //        }
        //        else
        //        {
        //            if (url == null)
        //            {
        //                var esiaUrl = await _miacAuthService.GetURL();
        //                if (esiaUrl == null)
        //                    return RedirectToAction("Index", "Auth");
        //                return Redirect(esiaUrl.url);
        //            }
        //            authUser = await _miacAuthService // вместо вызова авторизации через тестовое апи можно сделать возврат на ошибку и т.п.
        //                .AuthorizeUserFromTestApi(url); // по умолчанию админ
        //        }
        //        string roleName = _authService.GetRoleFromAuthUserDto(authUser);

        //        //get data from db
        //        var roles = _context.Roles.ToList();
        //        var userRole = roles.Where(r => r.Title == roleName).FirstOrDefault();

        //        if (userRole == null)
        //        {
        //            await Console.Out.WriteLineAsync("AuthController: userRole not found");
        //            return NotFound("AuthController: user role not found, access denied");
        //        }

        //        var groups = _context.Groups.ToList();
        //        var userGroup = groups.Where(g => g.Title == "Не определён").FirstOrDefault(); //изменить на выбранный отдел

        //        if (userGroup == null)
        //        {
        //            await Console.Out.WriteLineAsync("AuthController: userGroup not found");
        //            userGroup = groups.Where(g => g.Title == "Не определён").FirstOrDefault();
        //        }

        //        //mapping user
        //        User user = new()
        //        {
        //            Firstname = authUser.first_name,
        //            Surname = authUser.last_name,
        //            Patronymic = authUser.patronymic,
        //            Snils = authUser.snils,
        //            IdRoleNavigation = userRole,
        //            IdGroupNavigation = userGroup!,
        //            IsActive = false
        //        };

        //        //Saving user to database
        //        try
        //        {
        //            var existingUser = _context.Users
        //                .Include(u => u.IdRoleNavigation)
        //                .FirstOrDefault(u => u.Snils == user.Snils);

        //            if (existingUser == null)
        //            {
        //                _context.Add(user);
        //                await Console.Out.WriteLineAsync("user add");
        //            }
        //            else
        //            {
        //                existingUser.Firstname = user.Firstname;
        //                existingUser.Surname = user.Surname;
        //                existingUser.Patronymic = user.Patronymic;
        //                existingUser.IdRoleNavigation = user.IdRoleNavigation;
        //                existingUser.LastEnterDate = DateTime.Now;
        //                user.IsActive = existingUser.IsActive;
        //                _context.Update(existingUser);
        //                await Console.Out.WriteLineAsync("user update");
        //            }
        //            await _context.SaveChangesAsync();
        //            if (!user.IsActive && userRole.Title != "root")
        //            {
        //                return Ok("Ваша учётная запись требует активации. Обратитесь к системному администратору");
        //            }

        //            var userFromDb = await _context.Users
        //                .Include(u => u.IdGroupNavigation)
        //                .FirstOrDefaultAsync(u => u.Snils == user.Snils);

        //            if (userFromDb == null)
        //            {
        //                await Console.Out.WriteLineAsync("auth controller: userFromDb == null");
        //                return RedirectToAction("CustomError", "Home", new { errorText = "404😿" });
        //            }
        //            //SetUserToSession
        //            SetCurrentUserToSession(user);

        //            //Claims хранятся в куках
        //            var claims = new List<Claim>
        //        {
        //            new Claim("id", userFromDb.IdUser.ToString()),
        //            new Claim("group", userFromDb.IdGroupNavigation.Title.ToString()),
        //            new Claim(ClaimsIdentity.DefaultIssuer, user.Firstname),
        //            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Snils),
        //            new Claim(ClaimsIdentity.DefaultRoleClaimType, user.IdRoleNavigation.Title)
        //        };
        //            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        //            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        //            await HttpContext.SignInAsync(claimsPrincipal);
        //        }
        //        catch (Exception ex)
        //        {
        //            return NotFound(ex.Message);
        //        }
        //        return RedirectToAction("Index", "Dashboard");
        //    }

        //    catch (Exception ex)
        //    {
        //        await Console.Out.WriteLineAsync($"AuthController - Login - {ex.Message}");
        //        return RedirectToAction("CustomError", "Home", new { errorText = "500 Internal Server Error" });
        //    }
        //}

        public async Task<IActionResult> Exit()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Landing");
        }

        private void SetCurrentUserToSession(User user)
        {
            HttpContext.Session.Set<string>("currentUser", user.Snils);
        }
    }
}
