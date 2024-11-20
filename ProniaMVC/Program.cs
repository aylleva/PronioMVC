using Microsoft.EntityFrameworkCore;
using ProniaMVC.DAL;

namespace ProniaMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppDBContext>
                (option => option.UseSqlServer("server=DESKTOP-RLSUGQE\\SQLEXPRESS;database=Pronia;trusted_connection=true;integrated security=true;TrustServerCertificate=true;")
                );
            var app = builder.Build();
            app.UseStaticFiles();

            app.MapControllerRoute(

               "default",
               "{controller=home}/{action=index}/{Id?}"

                );

            app.Run();
        }
    }
}
