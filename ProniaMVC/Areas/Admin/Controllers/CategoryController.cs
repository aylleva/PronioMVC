using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.Areas.Admin.ViewModels;
using ProniaMVC.DAL;
using ProniaMVC.Models;

namespace ProniaMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDBContext _context;

        public CategoryController(AppDBContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<GetCategoryVM> categoryvm = await _context.Categories.Where(c=>c.IsDeleted!=true).Include(p => p.Product)
                .Select(c=> new GetCategoryVM { Id=c.Id,Name=c.Name,ProductCount=c.Product.Count})
                .ToListAsync();
            return View(categoryvm);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryVM categoryvm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool result = await _context.Categories.AnyAsync(c => c.Name == categoryvm.Name);
            if (result)
            {
                ModelState.AddModelError("Name", "This category name is already exists");
                return View();
            }
            Category category = new()
            {
                Name = categoryvm.Name,
                IsDeleted = false,
                DateTime = DateTime.Now
            };

            _context.Categories.Add(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();

            UpdateCategoryVM categoryvm = new() { Name=category.Name };

            return View(categoryvm);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int? id, CreateCategoryVM categoryvm)
        {
            if (id == null || id < 1) return BadRequest();

            Category existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (existed == null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View();
            }

            bool result = await _context.Categories.AnyAsync(c => c.Name.Trim() == categoryvm.Name.Trim() && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "This Category Name Is Already Exists!");
                return View();
            }
            existed.Name = categoryvm.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();

            category.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Category category=await _context.Categories.Include(c=>c.Product).ThenInclude(p=>p.ProductImages).FirstOrDefaultAsync(c=>c.Id==id);
            if (category == null) return NotFound();

            return View(category);
        }


    }
}
