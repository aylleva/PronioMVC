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
                .Take(10)
                .ToListAsync(),

                Products=await _context.Products
                .Take(8)
                .Include(img => img.ProductImages.Where(pi => pi.IsPrimary != null))
                .ToListAsync(), 

                NewProducts= await _context.Products
                .OrderByDescending(p=>p.DateTime)
                .Take(8)
                .Include(img => img.ProductImages.Where(pi => pi.IsPrimary != null))
                .ToListAsync()
                };
            return View(vm);  
        }

        public IActionResult Error(string errormessage)
        {
            return View(model:errormessage);
        }
    }


}
