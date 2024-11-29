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

        public async Task<IActionResult> Index()
        {
            
                HomeVM vm = new HomeVM {
                Slides = await _context.Slides
                .OrderBy(s=>s.Order)
                .Take(2)
                .ToListAsync(),

                Products=await _context.Products
                //.Take(7)
                .Include(img => img.ProductImages/*.Where(pi => pi.IsPrimary != null)*/)
                .ToListAsync(), 
            };
            return View(vm);  
        }
    }
}
