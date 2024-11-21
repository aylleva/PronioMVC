using Microsoft.EntityFrameworkCore;
using ProniaMVC.Models;

namespace ProniaMVC.DAL
{
    public class AppDBContext:DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options):base(options) { }
        
        public DbSet<Slide>Slides { get; set; }

        public DbSet<Product>products{ get; set; }
        public DbSet<ProductImage> productImages { get; set; }
        public DbSet<Category> categories { get; set; }
    }
}
