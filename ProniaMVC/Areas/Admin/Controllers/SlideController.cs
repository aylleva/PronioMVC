using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.DAL;
using ProniaMVC.Models;
using ProniaMVC.Utilitie.Extentions;


namespace ProniaMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _env;

        public SlideController(AppDBContext context,IWebHostEnvironment env)
        {
            _context = context;
           _env = env;
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
            if (!slide.Photo.CheckType("image/"))
            {
                ModelState.AddModelError("Photo", "Wrong Format!");
                return View();
            }
            if (!slide.Photo.CheckFileSize(Utilitie.Enums.FileSize.MB,2))
            {
                ModelState.AddModelError("Photo", "File length must contains max 2MB");
                return View();
            }

            slide.Image =await  slide.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
        

            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
           
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id < 1) return BadRequest();
            Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);

            if (slide == null) return NotFound();

            slide.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");

            _context.Slides.Remove(slide);
           await  _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }
    }
}
