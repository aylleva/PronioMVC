using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.DAL;
using ProniaMVC.Models;
using ProniaMVC.Utilitie.Enums;

namespace ProniaMVC.ViewComponents
{
    public class ProductViewComponent:ViewComponent
    {
        private readonly AppDBContext _context;

        public ProductViewComponent(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(SortType type)
        {
            List<Product> products = null;
            switch (type)
            
            { 
                case SortType.Name:
                    products = await _context.Products.OrderBy(p => p.Name)
                  .Take(8)
                  .Include(img => img.ProductImages.Where(pi => pi.IsPrimary != null))
                  .ToListAsync();
                    break;  
                case SortType.Price:
                   products = await _context.Products.OrderByDescending(p => p.Price)
                 .Take(8)
                 .Include(img => img.ProductImages.Where(pi => pi.IsPrimary != null))
                 .ToListAsync();
                    break;
                case SortType.Time:
                    products = await _context.Products.OrderByDescending(p => p.DateTime)
                .Take(8)
                .Include(img => img.ProductImages.Where(pi => pi.IsPrimary != null))
                .ToListAsync();
                    break;
            }
            return View(products);  


        }

    }
}
