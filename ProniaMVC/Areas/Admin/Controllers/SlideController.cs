using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.Areas.Admin.ViewModels;
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
        public readonly string root = Path.Combine("assets", "images", "website-images");
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
        public async Task<IActionResult> Create(CreateSlideVM slideVM)
        {
            if (!slideVM.Photo.CheckType("image/"))
            {
                ModelState.AddModelError("Photo", "Wrong Format!");
                return View();
            }
            if (!slideVM.Photo.CheckFileSize(Utilitie.Enums.FileSize.MB,2))
            {
                ModelState.AddModelError("Photo", "File length must contains max 2MB");
                return View();
            }

            Slide slide = new Slide { 
            Title=slideVM.Title,
            SubTitle=slideVM.SubTitle,
            Description=slideVM.Description,
            Image = await slideVM.Photo.CreateFileAsync(_env.WebRootPath,root),
            IsDeleted=false,
            DateTime=DateTime.Now



            };




        
        

            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
           
        }


        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Slide slide= await _context.Slides.FirstOrDefaultAsync(s=>s.Id== id); 
            if (slide == null) return NotFound();

            UpdateSlideVM slideVM = new UpdateSlideVM { 
            
            Title=slide.Title,
            SubTitle=slide.SubTitle,
            Description=slide.Description,
            Image=slide.Image,
            Order=slide.Order,
            
            };
            return View(slideVM);   
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateSlideVM slideVM)
        {
            if (!ModelState.IsValid)
            {
                return View(slideVM);
            }
            Slide existed = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (existed == null) return NotFound();

            if (slideVM.Photo is not null)
            {
                if (!slideVM.Photo.CheckType("image/"))
                {
                    ModelState.AddModelError("Photo", "Wrong Format!");
                    return View();
                }
                if (!slideVM.Photo.CheckFileSize(Utilitie.Enums.FileSize.MB, 2))
                {
                    ModelState.AddModelError("Photo", "File length must contains max 2MB");
                    return View();
                }

                string filename = await slideVM.Photo.CreateFileAsync(_env.WebRootPath, root);

               existed.Image.DeleteFile(_env.WebRootPath,root);
                existed.Image = filename;

            }

            existed.Title = slideVM.Title;
            existed.Description = slideVM.Description;
            existed.SubTitle = slideVM.SubTitle;
            existed.Order = slideVM.Order;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id < 1) return BadRequest();
            Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);

            if (slide == null) return NotFound();

            slide.Image.DeleteFile(_env.WebRootPath, root);

            _context.Slides.Remove(slide);
           await  _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }
    }
}
