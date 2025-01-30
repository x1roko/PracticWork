using DinkToPdf.Contracts;
using DinkToPdf;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TaskManagerMiac.Data;
using TaskManagerMiac.Service;
using TaskManagerMiac.Models;

namespace TaskManagerMiac
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            //builder.Services.AddDbContext<AppDbContext>(options =>
            //{
            //    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnectionMysql"));
            //});
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnectionPostgree"));
            });


            builder.Services.AddHttpClient();

            builder.Services.AddScoped<AuthService>();
            builder.Services.AddSingleton<MIACAuthService>();
            builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            builder.Services.AddScoped<DocumentsService>();
            //Sessions
            builder.Services.AddDistributedMemoryCache();// добавляем IDistributedMemoryCache
            //в сессии хранится снилс текущего пользователя
            var sessionTimeoutMinutes = 10;
            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = "currentUser";
                options.IdleTimeout = TimeSpan.FromMinutes(sessionTimeoutMinutes); // выход из сессии после sessionTimeoutMinutes минут бездействия
                options.Cookie.IsEssential = true;
                options.IOTimeout = TimeSpan.FromMinutes(sessionTimeoutMinutes);
            });

            //Cookie для ролей пользователей
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login";
                    options.AccessDeniedPath = "/Home/Privacy";
                });
            builder.Services.AddAuthorization();

            var app = builder.Build();

            //Sessions
            app.UseSession();   // добавляем middleware для работы с сессиями

            //Authorization for roles
            app.UseAuthentication();
            app.UseAuthorization();   // добавление middleware авторизации 

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Landing}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
