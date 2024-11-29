using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.Areas.Admin.ViewModels;

using ProniaMVC.DAL;
using ProniaMVC.Models;

namespace ProniaMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<GetProductVM> productVm = await _context.Products
                  .Include(p => p.Category)
                  .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                  .Select(p => new GetProductVM
                  {
                      Name = p.Name,
                      Price = p.Price,
                      CategoryName = p.Category.Name,
                      Image = p.ProductImages[0].Image
                  }



                  )
                  .ToListAsync();

            return View(productVm);
        }

        public async Task<IActionResult> Create()
        {
            CreateProductVM productvm = new CreateProductVM
            {
                Tags=await _context.Tags.ToListAsync(),
                Categories = await _context.Categories.ToListAsync()
            };

            return View(productvm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productvm)
        {
            productvm.Categories = await _context.Categories.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View(productvm);
            }

            bool result = await _context.Categories.AnyAsync(c => c.Id == productvm.CategoryId);
            if (!result)
            {
                ModelState.AddModelError(nameof(CreateProductVM.CategoryId), "Category does not exists!");
                return View(productvm);
            }
            if (productvm.Tagid is not null)
            {
                bool tagresult = productvm.Tags.Any(t => !productvm.Tags.Exists(pt => pt.Id == t.Id));
                if (tagresult)
                {
                    ModelState.AddModelError(nameof(CreateProductVM.Tagid), "Incorrect");
                    return View(productvm);
                }
            }
            Product product = new Product
            {
                Name = productvm.Name,
                Price = productvm.Price.Value,
                Description = productvm.Description,
                CategoryId = productvm.CategoryId.Value,
                SKU = productvm.SKU,
                IsDeleted = false,
                DateTime = DateTime.Now

            };
            if(productvm.Tagid != null)
            {
               product.Tags=productvm.Tags.Select(t=>new ProductTags {TagId=t.Id}).ToList();
            }
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
           return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == 0 || id < 1) return BadRequest();

            Product product = await _context.Products.Include(p=>p.Tags).FirstOrDefaultAsync(c => c.Id == id);
            if (product == null) return NotFound();


            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                SKU = product.SKU,
                CategoryId = product.CategoryId,
                Categories = await _context.Categories.ToListAsync(),
                Tags=await _context.Tags.ToListAsync(),
                TagIds=product.Tags.Select(t=>t.TagId).ToList()
                
            };
            return View(productVM);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int? id, UpdateProductVM productVM)
        {
            if (id == 0 || id < 1) return BadRequest();

            Product existed = await _context.Products.FirstOrDefaultAsync(c => c.Id == id);
            if (existed == null) return NotFound();

            productVM.Categories = await _context.Categories.ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(productVM);
            }

            if (existed.CategoryId != productVM.CategoryId)
            {
                bool result = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
                if (!result)
                {
                    ModelState.AddModelError(nameof(Category.Id), "This category does not exists!");
                    return View(productVM);
                }
            }

            existed.SKU = productVM.SKU;
            existed.Name = productVM.Name;
            existed.Description = productVM.Description;
            existed.Price=productVM.Price.Value;
            existed.CategoryId=productVM.CategoryId.Value;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
