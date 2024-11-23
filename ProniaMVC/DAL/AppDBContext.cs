using Microsoft.EntityFrameworkCore;
using ProniaMVC.Models;

namespace ProniaMVC.DAL
{
    public class AppDBContext:DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options):base(options) { }
        
        public DbSet<Slide>Slides { get; set; }

<<<<<<< HEAD
        public DbSet<Product> Products{ get; set; }
=======
        public DbSet<Product>Products{ get; set; }
>>>>>>> a92e36ab4a78f4392877a20ce9dc18ef84610615
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
