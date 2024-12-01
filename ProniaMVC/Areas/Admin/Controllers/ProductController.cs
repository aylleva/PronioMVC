using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.Areas.Admin.ViewModels;

using ProniaMVC.DAL;
using ProniaMVC.Models;
using ProniaMVC.Models.Base;
using ProniaMVC.Utilitie.Extentions;

namespace ProniaMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _env;
        public readonly string root = Path.Combine("assets", "images", "website-images");


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
                      Id = p.Id,
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
                Colors=await _context.Colors.ToListAsync(),
                Sizes=await _context.Sizes.ToListAsync(), 
                Tags=await _context.Tags.ToListAsync(),
                Categories = await _context.Categories.ToListAsync()
            };

            return View(productvm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productvm)
        {
            productvm.Colors=await _context.Colors.ToListAsync();
            productvm.Sizes=await _context.Sizes.ToListAsync();
            productvm.Tags = await _context.Tags.ToListAsync();
            productvm.Categories = await _context.Categories.ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(productvm);
            }

            if (!productvm.MainPhoto.CheckType("image/"))
            {
                ModelState.AddModelError(nameof(productvm.MainPhoto), "Wrong Type!");
                return View(productvm);
            }
            if (productvm.MainPhoto.CheckFileSize(FileSize.MB, 2))
            {

                ModelState.AddModelError(nameof(productvm.MainPhoto), "Wrong Size!");
                return View(productvm);
            }

            if (!productvm.HoverPhoto.CheckType("image/"))
            {
                ModelState.AddModelError(nameof(productvm.HoverPhoto), "Wrong Type!");
                return View(productvm);
            }
            if (productvm.HoverPhoto.CheckFileSize(FileSize.MB, 2))
            {

                ModelState.AddModelError(nameof(productvm.HoverPhoto), "Wrong Size!");
                return View(productvm);
            }

            bool result = await _context.Categories.AnyAsync(c => c.Id == productvm.CategoryId);
            if (!result)
            {
                ModelState.AddModelError(nameof(CreateProductVM.CategoryId), "Category does not exists!");
                return View(productvm);
            }
            if (productvm.Tagids is not null)
            {
                bool tagresult = productvm.Tagids.Any(t => !productvm.Tags.Exists(pt => pt.Id == t));
                if (tagresult)
                {
                    ModelState.AddModelError(nameof(CreateProductVM.Tagids), "Incorrect");
                    return View(productvm);
                }
            }
             if(productvm.ColorIds is not null)
            {
                bool colorresult = productvm.ColorIds.Any(ci => !productvm.Colors.Exists(c => c.Id == ci));
                if (colorresult)
                {
                    ModelState.AddModelError(nameof(CreateProductVM.ColorIds), "Incorrect");
                    return View(productvm);
                }
            }
            if (productvm.SizeIds is not null)
            {
                bool colorresult = productvm.SizeIds.Any(ci => !productvm.Sizes.Exists(c => c.Id == ci));
                if (colorresult)
                {
                    ModelState.AddModelError(nameof(CreateProductVM.SizeIds), "Incorrect");
                    return View(productvm);
                }
            }

            ProductImage main = new()
            {
                Image = await productvm.MainPhoto.CreateFileAsync(root),
                IsDeleted = false,
                IsPrimary = true,
                DateTime = DateTime.Now
            };
            ProductImage hover = new()
            {
                Image = await productvm.HoverPhoto.CreateFileAsync(root),
                IsDeleted = false,
                IsPrimary = true,
                DateTime = DateTime.Now
            };



            Product product = new Product
            {
                Name = productvm.Name,
                Price = productvm.Price.Value,
                Description = productvm.Description,
                CategoryId = productvm.CategoryId.Value,
                SKU = productvm.SKU,
                IsDeleted = false,
                DateTime = DateTime.Now,
                ProductImages=new List<ProductImage> { main,hover }

            };
            if(productvm.Tagids != null)
            {
               product.Tags=productvm.Tags.Select(t=>new ProductTags {TagId=t.Id}).ToList();
            }

            if(productvm.Colors != null)
            {
                product.ProductColors=productvm.Colors.Select(c=>new ProductColors { ColorId=c.Id}).ToList();
            }
            if (productvm.Sizes != null)
            {
                product.ProductSizes= productvm.Sizes.Select(s => new ProductSizes { SizeId = s.Id }).ToList();
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
           return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id==null|| id < 1) return BadRequest();

            Product product = await _context.Products.Include(p=>p.Tags).Include(p=>p.ProductColors).Include(p=>p.ProductSizes).FirstOrDefaultAsync(c => c.Id == id);
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
                TagIds=product.Tags.Select(t=>t.TagId).ToList(),

                Colors=await _context.Colors.ToListAsync(),
                ColorIds=product.ProductColors.Select(pc=>pc.ColorId).ToList(),

                Sizes=await _context.Sizes.ToListAsync(),
                SizeIds=product.ProductSizes.Select(pc=>pc.SizeId).ToList()   
                
            };
            return View(productVM);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int? id, UpdateProductVM productVM)
        {
            if (id == 0 || id < 1) return BadRequest();

            productVM.Colors = await _context.Colors.ToListAsync();
            productVM.Sizes= await _context.Sizes.ToListAsync();
            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();

            Product existed = await _context.Products.Include(p=>p.Tags).Include(p=>p.ProductColors).Include(p=>p.ProductSizes).FirstOrDefaultAsync(c => c.Id == id);
            if (existed == null) return NotFound();

           
       
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
         
            if (productVM.TagIds is not null)
            {
                bool tagresult = productVM.TagIds.Any(t => !productVM.Tags.Exists(pt => pt.Id ==t));
                if (tagresult)
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.TagIds), "Incorrect");
                    return View(productVM);
                }
            }

            if(productVM.ColorIds is not null)
            {
                bool colorresult = productVM.ColorIds.Any(ci => !productVM.Colors.Exists(c => c.Id == ci));
                if(colorresult)
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.ColorIds), "Incorrect");
                    return View(productVM);
                }
            }

            if (productVM.SizeIds is not null)
            {
                bool sizeresult = productVM.SizeIds.Any(sI => !productVM.Sizes.Exists(s => s.Id == sI));
                if (sizeresult)
                {
                    {
                        ModelState.AddModelError(nameof(UpdateProductVM.SizeIds), "Incorrect");
                        return View(productVM);
                    }
                }
            }


            if (productVM.ColorIds is  null)
            {
                productVM.ColorIds = new();
            }
           

            _context.ProductColors.RemoveRange(existed.ProductColors
              .Where(pc => !productVM.ColorIds
              .Exists(cI => cI == pc.ColorId)).ToList());

            _context.ProductColors.AddRange(productVM.ColorIds
            .Where(cI => !existed.ProductColors
            .Exists(pc => pc.ColorId == cI))
            .Select(c => new ProductColors { ColorId = c, ProductId = existed.Id }).ToList());

         
           

            if (productVM.SizeIds is  null)
            {
                productVM.SizeIds = new();
            }
           

            _context.ProductSizes
              .RemoveRange(existed.ProductSizes
              .Where(pc => !productVM.SizeIds
              .Exists(cI => cI == pc.SizeId)).ToList());



            _context.ProductSizes
             .AddRange(productVM.SizeIds
            .Where(cI => !existed.ProductSizes
            .Exists(pc => pc.SizeId == cI))
            .Select(c => new ProductSizes { SizeId = c, ProductId = existed.Id }).ToList());

           


            if (productVM.TagIds is null)
                {
                    productVM.TagIds = new();
                }
                else
                {
                    productVM.TagIds = productVM.TagIds.Distinct().ToList();
                }


                _context.ProductTags
                .RemoveRange(existed.Tags
                .Where(t => !productVM.TagIds
                .Exists(ti => ti == t.TagId)).ToList());

                _context.ProductTags
                .AddRange(productVM.TagIds
                .Where(ti => !existed.Tags
                .Exists(t => t.TagId == ti))
                .Select(ti => new ProductTags { TagId = ti, ProductId = existed.Id }).ToList());

           


         

                existed.SKU = productVM.SKU;
                existed.Name = productVM.Name;
                existed.Description = productVM.Description;
                existed.Price = productVM.Price.Value;
                existed.CategoryId = productVM.CategoryId.Value;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
