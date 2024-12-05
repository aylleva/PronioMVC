using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.PointsToAnalysis;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.Areas.Admin.ViewModels;
using ProniaMVC.DAL;
using ProniaMVC.Models;

namespace ProniaMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TagController : Controller
    {
        private readonly AppDBContext _context;
      

        public TagController(AppDBContext context)
        {
           _context = context;
          
        }
        public async Task<IActionResult> Index()
        
        {
           List<GetTagVM> tagVM = await _context.Tags
             .Include(t=>t.Tags)
             .Select(t=> new GetTagVM 
             {
                 Id=t.Id,   
                 Name=t.Name,
                
                 

             }).ToListAsync();

                
            return View(tagVM);
        }

        public IActionResult Create()
        {
            return View();  
        }

        [HttpPost]

        public async Task<IActionResult> Create(CreateTagVM tagVM)
        {
            if(!ModelState.IsValid)
            {
                return View(tagVM);
            }
            bool result = await _context.Tags.AnyAsync(t => t.Name == tagVM.Name);
            if(result)
            {
                ModelState.AddModelError(nameof(CreateTagVM.Name), "This Tag is already exists!!");
                return View(tagVM);
            }
            Tag tag= new Tag
            { 
             Name=tagVM.Name,
             IsDeleted=false,
             DateTime = DateTime.Now
            };
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id < 1) return BadRequest();

            Tag tag=await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (tag == null) return NotFound();

            UpdateTagVM tagvm = new UpdateTagVM { Name = tag.Name };

            return View(tagvm); 
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id,UpdateTagVM tagvm)
        {
            if (id is null || id < 1) return BadRequest();

            Tag existed = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (existed == null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(tagvm);
            }

            bool result=await _context.Tags.AnyAsync(t=>t.Name.Trim() == tagvm.Name.Trim() && t.Id!=id);
            if (result)
            {
                ModelState.AddModelError(nameof(UpdateTagVM.Name), "This tag is already exists!");
                return View(tagvm);
            }
            existed.Name=tagvm.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id < 1) return BadRequest();
            Tag tag = await _context.Tags.FirstOrDefaultAsync(tag => tag.Id==id);
            if (tag == null) return NotFound();

            tag.IsDeleted = true;
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
