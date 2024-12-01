using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.DAL;
using ProniaMVC.Models;
using ProniaMVC.ViewModels;

namespace ProniaMVC.Controllers
{
    public class ShopController:Controller

    {
        private readonly AppDBContext _context;

        public ShopController(AppDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Product? product =await _context.Products
                .Include(p=>p.ProductImages.OrderByDescending(p=>p.IsPrimary))
                .Include(p=>p.Category)
                .Include(p=>p.Tags)
                .ThenInclude(t=>t.Tag)
                .Include(p=>p.ProductColors)
                .ThenInclude(pc=>pc.Color)
                .Include(p=>p.ProductSizes)
                .ThenInclude(ps=>ps.Size)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();

            DetailVM detailVM = new DetailVM
            {
                Product = product,
                RelatedProducts =await _context.Products
                .Where(p => p.CategoryId == product.CategoryId && p.Id != id)
                .Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary!=null)) 
                .Take(8)
                .ToListAsync()
            };

            return View(detailVM);
        }
    }
}
