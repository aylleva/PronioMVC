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

        public IActionResult Detail(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Product? product = _context.Products
                .Include(p=>p.ProductImages.OrderByDescending(p=>p.IsPrimary))
                .Include(p=>p.Category)
                .FirstOrDefault(p => p.Id == id);

            if (product is null) return NotFound();

            DetailVM detailVM = new DetailVM
            {
                Product = product,
                RelatedProducts = _context.Products
                .Where(p => p.CategoryId == product.CategoryId && p.Id != id)
                .Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary!=null)) 
                .ToList()
            };

            return View(detailVM);
        }
    }
}
