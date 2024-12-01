using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.Areas.Admin.ViewModels.Sizes;
using ProniaMVC.DAL;
using ProniaMVC.Models.Base;

namespace ProniaMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SizeController : Controller
    {
        private readonly AppDBContext context;

        public SizeController(AppDBContext context)
        {
            this.context = context;
        }
        public async Task<IActionResult> Index()
        {
         List<GetSizeVM> sizevm =await context.Sizes
         .Include(s=>s.ProductSizes)
         .Select(s=> new GetSizeVM
         { 
         Name = s.Name,
         id = s.Id,
         
         
         } ).ToListAsync();
            return View(sizevm);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(CreateSizeVM sizeVM)
        {
            if(!ModelState.IsValid)
            {
                return View(sizeVM);
            }

            bool result=await context.Sizes.AnyAsync(s=>s.Name== sizeVM.Name);
            if(result)
            {
                ModelState.AddModelError(nameof(CreateSizeVM.Name), "This Size is already exists!");
                return View(sizeVM);
            }

            Size size = new()
            {
                Name= sizeVM.Name,
                IsDeleted= false,
                DateTime = DateTime.Now
            };

            await context.Sizes.AddAsync(size);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if(id is null || id < 1) return BadRequest();

            Size size=await context.Sizes.FirstOrDefaultAsync(s=>s.Id==id);
            if(size is null) return NotFound();

            UpdateSizeVM sizeVM = new() { Name = size.Name };

            return View(sizeVM);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int? id, UpdateSizeVM sizeVM)
        {
            if (id is null || id<1) return BadRequest();

            Size existed= await context.Sizes.FirstOrDefaultAsync(s=>s.Id==id);
            if(existed is null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(sizeVM);
            }

            bool result=await context.Sizes.AnyAsync(s=>s.Name.Trim()==sizeVM.Name.Trim() && s.Id!=id);
            if (result)
            {
                ModelState.AddModelError(nameof(UpdateSizeVM.Name), "This Size is already exists!");
                return View(sizeVM);
            }

            existed.Name=sizeVM.Name;

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
