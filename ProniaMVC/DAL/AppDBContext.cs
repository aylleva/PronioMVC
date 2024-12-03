using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.Models;
using ProniaMVC.Models.Base;

namespace ProniaMVC.DAL
{
    public class AppDBContext:IdentityDbContext<AppUser>
    {
        public AppDBContext(DbContextOptions<AppDBContext> options):base(options) { }
        
        public DbSet<Slide>Slides { get; set; }

        public DbSet<Product> Products{ get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProductTags> ProductTags { get; set; }

        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductColors> ProductColors { get; set; }
        public DbSet<Size> Sizes { get; set; }  
        public DbSet<ProductSizes> ProductSizes { get; set; }
    }
}
