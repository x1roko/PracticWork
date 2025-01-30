using Microsoft.EntityFrameworkCore;
using TaskManagerMiac.Data;
using TaskManagerMiac.Extensioin;
using TaskManagerMiac.Models;

namespace TaskManagerMiac.Service
{
    /// <summary>
    /// Авторизация пользователя и выдача роли
    /// </summary>
    public class AuthService
    {
        private string _oidMiac = "1.2.643.5.1.13.13.12.2.29.2692";
        private string _appName;
        private readonly IConfiguration _configuration;

        private readonly AppDbContext _context;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _appName = Environment.GetEnvironmentVariable("APP_NAME") ?? _configuration["AppName"] ?? "";
        }

        /// <summary>
        /// Получить роль пользователя из сессии
        /// </summary>
        /// <param name="session"></param>
        /// <returns>
        /// root - все права
        /// admin - руководитель
        /// group_admin - руководитель отдела
        /// default - обычный пользователь
        /// unauthorized - неавторизованный пользователь
        /// </returns>
        public string GetUserRoleFromSession(ISession session)
        {
            var userRole = "unauthorized";
            User currentUser = GetUserFromSession(session);
            if (currentUser == null)
            {
                return userRole;
            }

            userRole = GetRoleFromUser(currentUser);

            return userRole;
        }

        /// <summary>
        /// Получить роль из AuthUserDto (json объект)
        /// </summary>
        /// <param name="authUser"></param>
        /// <returns>
        /// root - все права
        /// admin - руководитель
        /// group_admin - руководитель отдела
        /// default - обычный пользователь
        /// unauthorized - неавторизованный пользователь
        /// </returns>
        public string GetRoleFromAuthUserDto(AuthUserDto authUser)
        {
            var userRole = "unauthorized";
            if (authUser == null)
            {
                Console.WriteLine("AuthService: authUser is null");
                return userRole;
            }
            var positions = authUser.positions;
            if (positions.FirstOrDefault(p => p.hospital.oid == _oidMiac) == null) // если пользователя нет в миац
            {
                Console.WriteLine("AuthService: user isnt work in miac");
                return userRole;
            }
            var position = positions.FirstOrDefault(p => p.hospital.oid == _oidMiac);

            if (position == null || position.access_rights.Count == 0) //нет прав
            {
                Console.WriteLine("AuthService: access rights is empty");
                return userRole;
            }
            if (position.access_rights.FirstOrDefault(p => p.app_name == _appName) == null) // нет нашего приложения
            {
                Console.WriteLine("AuthService: task manager isnt in access rights");
                return userRole;
            }

            var accessRight = position.access_rights.FirstOrDefault(p => p.app_name == _appName);
            userRole = accessRight.role_name;

            if (string.IsNullOrEmpty(userRole))
            {
                Console.WriteLine("AuthService: user role is empty string");
                return userRole;
            }

            return userRole;
        }

        /// <summary>
        /// Получить роль из User (database model)
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns>
        /// root - все права
        /// admin - руководитель
        /// group_admin - руководитель отдела
        /// default - обычный пользователь
        /// unauthorized - неавторизованный пользователь
        /// </returns>
        public string GetRoleFromUser(User currentUser)
        {
            var userRole = "unauthorized";

            if (currentUser == null)
            {
                return userRole;
            }
            if (currentUser.IdRoleNavigation == null)
            {
                return userRole;
            }
            return currentUser.IdRoleNavigation.Title;
        }

        /// <summary>
        /// получаем текущего User из данных сессии
        /// </summary>
        /// <param name="session">HttpContext.Session</param>
        /// <returns>
        /// User
        /// </returns>
        public User? GetUserFromSession(ISession session)
        {
            if (!session.Keys.Contains("currentUser")) //юзера нет в сессии
            {
                Console.WriteLine("AuthService: юзера нет в сессии");
                return null;
            }

            string currentUserSnils = session.Get<string>("currentUser");
            if (currentUserSnils == null) //auth user is null
            {
                Console.WriteLine("AuthService: currentUserSnils is null");
            }

            var currentUser = _context.Users
                .Include(u => u.IdRoleNavigation)
                .Include(u => u.IdRoleNavigation)
                .AsNoTracking()
                .FirstOrDefault(u => u.Snils == currentUserSnils);

            return currentUser;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns>
        /// true - user авторизован
        /// false - user не авторизован и имеет роль "unauthorized"
        /// </returns>
        public bool IsAuthorizedUser(User currentUser)
        {
            return GetRoleFromUser(currentUser) != "unauthorized";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns>
        /// true - user авторизован
        /// false - user не авторизован и имеет роль "unauthorized"
        /// </returns>
        public bool IsAuthorizedUser(ISession session)
        {
            var user = GetUserFromSession(session);
            //if (user != null )//&& user.LastEnterDate.AddHours(3) < DateTime.Now)
            //{
            //    session.Clear();
            //}
            return GetUserRoleFromSession(session) != "unauthorized";
        }
    }
}
