using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
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
            List<Category> categories = await _context.Categories.Where(c=>c.IsDeleted!=true).Include(p => p.Product).ToListAsync();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool result = await _context.Categories.AnyAsync(c => c.Name == category.Name);
            if (result)
            {
                ModelState.AddModelError("Name", "This category name is already exists");
                return View();
            }
            category.DateTime = DateTime.Now;

            _context.Categories.Add(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int? id, Category category)
        {
            if (id == null || id < 1) return BadRequest();

            Category existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (existed == null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View();
            }

            bool result = await _context.Categories.AnyAsync(c => c.Name.Trim() == category.Name.Trim() && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "This Category Name Is Already Exists!");
                return View();
            }
            existed.Name = category.Name;

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

            Category category=await _context.Categories.FirstOrDefaultAsync(c=>c.Id==id);
            if (category == null) return NotFound();
            return View(category);
        }


    }
}
