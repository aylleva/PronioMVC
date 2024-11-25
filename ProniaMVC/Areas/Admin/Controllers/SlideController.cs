using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.DAL;
using ProniaMVC.Models;

namespace ProniaMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController : Controller
    {
        private readonly AppDBContext _context;

        public SlideController(AppDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Slide> slides=_context.Slides.OrderBy(s=>s.Order).ToList();
            return View(slides);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Slide slide)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

           await _context.Slides.AddAsync(slide);
           await _context.SaveChangesAsync();  

            return RedirectToAction(nameof(Index));
           
        }
    }
}
