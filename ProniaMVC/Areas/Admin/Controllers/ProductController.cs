using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.Areas.Admin.ViewModels;
using ProniaMVC.DAL;

namespace ProniaMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDBContext context,IWebHostEnvironment env)
        {
            _context = context;
          _env = env;
        }
        public async Task<IActionResult> Index()
        {
          List<GetProductVM> productVm=await _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary==true))
                .Select(p => new GetProductVM
                { 
                Name = p.Name,
                Price = p.Price,
                CategoryName=p.Category.Name,
                Image=p.ProductImages[0].Image
                }



                )
                .ToListAsync();
            
            return View(productVm);
        }

        public async  Task<IActionResult> Create()
        {
            CreateProductVM productvm = new CreateProductVM
            {

                Categories = await _context.Categories.ToListAsync()
            };

            return View(productvm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productvm)
        {
            productvm.Categories = await _context.Categories.ToListAsync();
            if(!ModelState.IsValid)
            {
                return View(productvm);
            }

            bool result= await _context.Categories.AnyAsync(c=>c.Id==productvm.CategoryId);
            if(!result)
            {
                ModelState.AddModelError(nameof(CreateProductVM.CategoryId), "Category does not exists!");
                return View(productvm);
            }
            return Content("slm");
        }
    }
}
