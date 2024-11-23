using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.DAL;
using ProniaMVC.Models;
using ProniaMVC.ViewModels;

namespace ProniaMVC.Controllers
{
    public class HomeController:Controller
    {
        public readonly AppDBContext _context;
        public HomeController(AppDBContext context)
        {
            _context = context;     
        }

        public IActionResult Index()
        {
            
                HomeVM vm = new HomeVM {
                Slides = _context.Slides
                .OrderBy(s=>s.Order)
                .Take(2)
                .ToList(),

                Products=_context.Products
                .Take(8)
                .Include(img=>img.ProductImages.Where(pi=>pi.IsPrimary!=null)).ToList(), 
            };
            return View(vm);  
        }
    }
}
