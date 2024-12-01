using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.Areas.Admin.ViewModels;
using ProniaMVC.DAL;
using ProniaMVC.Models.Base;

namespace ProniaMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ColorController : Controller
    {
        private readonly AppDBContext _context;

        public ColorController(AppDBContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<GetColorVM> colorvm =await _context.Colors
            .Include(c => c.ProductColors)
            .Select(pc => new GetColorVM
            { 
             id = pc.Id,
             Name = pc.Name,
            
            })
            .ToListAsync();
                
                
                return View(colorvm);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateColorVM colorvm)
        {
            if(!ModelState.IsValid)
            {
                return View(colorvm);
            }

            bool result=await _context.Colors.AnyAsync(c=>c.Name== colorvm.Name);
            if(result)
            {
                ModelState.AddModelError(nameof(CreateColorVM.Name), "This Color is already exists!");
                return View(colorvm);
            }

            Color color = new() {
                Name=colorvm.Name,
                IsDeleted=false,
                DateTime=DateTime.Now
                
            };

            await _context.Colors.AddAsync(color);
            await _context.SaveChangesAsync();
         return  RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Update(int? Id)
        {
            if(Id is null || Id<1) return BadRequest();
            Color color = await _context.Colors.FirstOrDefaultAsync(c=>c.Id== Id);
            if(color is null) return NotFound();

            UpdateColorVM colorvm = new()
            {
                Name = color.Name
            };
            return View(colorvm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateColorVM colorvm)
        {
            if (id is null || id < 1) return BadRequest();
            Color existed = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(colorvm);
            }

            bool result = await _context.Colors.AllAsync(c => c.Name == colorvm.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError(nameof(UpdateColorVM.Name), "This Color is already exists!");
                return View(colorvm);
            }

            existed.Name = colorvm.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }    
    }
}
