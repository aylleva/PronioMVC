using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.DAL;
using ProniaMVC.Middlewares;
using ProniaMVC.Models;
using ProniaMVC.Services.Implementations;
using ProniaMVC.Services.Interfaces;

namespace ProniaMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppDBContext>(opt=>
            
            opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
            
            );

            builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 8;
                opt.Password.RequireNonAlphanumeric = false;

                
                opt.User.RequireUniqueEmail = true;

                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                opt.Lockout.MaxFailedAccessAttempts = 3;
                opt.Lockout.AllowedForNewUsers = true;
            }
            ).AddEntityFrameworkStores<AppDBContext>().AddDefaultTokenProviders();

            builder.Services.AddScoped<ILayoutServices,LayoutServices>();
            builder.Services.AddScoped<IBasketServices,BasketServices>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            var app = builder.Build();
            app.UseStaticFiles();


            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseMiddleware<GlobalExceptionHandler>();

            app.MapControllerRoute(

            "admin",
            "{area:exists}/{controller=home}/{action=index}/{Id?}"

             );

            app.MapControllerRoute(

               "default",
               "{controller=home}/{action=index}/{Id?}"

                );

            app.Run();
        }
    }
}
