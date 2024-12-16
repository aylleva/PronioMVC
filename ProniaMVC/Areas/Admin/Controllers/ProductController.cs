using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using ProniaMVC.Areas.Admin.ViewModels;
using ProniaMVC.Areas.Admin.ViewModels.Product;
using ProniaMVC.DAL;
using ProniaMVC.Models;
using ProniaMVC.Models.Base;
using ProniaMVC.Utilitie.Extentions;

namespace ProniaMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
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
        public async Task<IActionResult> Index(int page=1)
        {
            if(page<1) return BadRequest();

            int count=await _context.Products.CountAsync();
            double total = Math.Ceiling((double)count / 2);

            if (page > total) return BadRequest();


            PaginatedItemVM<GetProductVM> productvm = new() {

                TotalPage = total,
                CurrectPage = page,
                Items = await _context.Products
                  .Skip((page - 1) * 2)
                  .Take(2)
                  .Include(p => p.Category)
                  .Include(p => p.ProductImages)
                  .Select(p => new GetProductVM
                  {
                      Id = p.Id,
                      Name = p.Name,
                      Price = p.Price,
                      CategoryName = p.Category.Name,
                      Image = p.ProductImages.FirstOrDefault(p => p.IsPrimary == true).Image
                  }



                  ).ToListAsync()


            };


            return View(productvm);
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
            if (!productvm.MainPhoto.CheckFileSize(FileSize.MB, 2))
            {

                ModelState.AddModelError(nameof(productvm.MainPhoto), "Wrong Size!");
                return View(productvm);
            }

            if (!productvm.HoverPhoto.CheckType("image/"))
            {
                ModelState.AddModelError(nameof(productvm.HoverPhoto), "Wrong Type!");
                return View(productvm);
            }
            if (!productvm.HoverPhoto.CheckFileSize(FileSize.MB, 2))
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
                Image = await productvm.MainPhoto.CreateFileAsync(_env.WebRootPath,root),
                IsDeleted = false,
                IsPrimary = true,
                DateTime = DateTime.Now
            };
            ProductImage hover = new()
            {
                Image = await productvm.HoverPhoto.CreateFileAsync(_env.WebRootPath, root),
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
               product.Tags=productvm.Tagids.Select(t=>new ProductTags {TagId=t}).ToList();
            }

            if(productvm.ColorIds != null)
            {
                product.ProductColors=productvm.ColorIds.Select(c=>new ProductColors { ColorId=c}).ToList();
            }
            if (productvm.SizeIds != null)
            {
                product.ProductSizes= productvm.SizeIds.Select(s => new ProductSizes { SizeId = s}).ToList();
            }

            if(productvm.AdditionalPhotos is not null)
            {
                string text = string.Empty;

                foreach(IFormFile file in productvm.AdditionalPhotos)
                {
                    if (!file.CheckType("image/"))
                    {
                        text += $"<div class=\"alert alert-warning\" role=\"alert\">\r\n {file.FileName} type is not correct!!\r\n</div>";
                        continue;
                    }
                    if (!file.CheckFileSize(FileSize.MB, 2))
                    {
                        text += $"<div class=\"alert alert-warning\" role=\"alert\">\r\n {file.FileName} size is not correct!!\r\n</div>";
                        continue;
                    }

                    product.ProductImages.Add(new ProductImage
                    {
                        Image = await file.CreateFileAsync(_env.WebRootPath,root),
                        IsDeleted = false,
                        DateTime=DateTime.Now,
                        IsPrimary=null

                    });
                }
                TempData["FileWarning"] = text;
            }
           

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
           return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id==null|| id < 1) return BadRequest();

            Product product = await _context.Products.Include(p=>p.ProductImages).Include(p=>p.Tags).Include(p=>p.ProductColors).Include(p=>p.ProductSizes).FirstOrDefaultAsync(c => c.Id == id);
            if (product == null) return NotFound();


            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                SKU = product.SKU,
                CategoryId = product.CategoryId,
                Categories = await _context.Categories.ToListAsync(),
                ProductImages=product.ProductImages,

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
            Product existed = await _context.Products.Include(p => p.ProductImages).Include(p => p.Tags).Include(p => p.ProductColors).Include(p => p.ProductSizes).FirstOrDefaultAsync(c => c.Id == id);
            if (existed == null) return NotFound();


            productVM.Colors = await _context.Colors.ToListAsync();
            productVM.Sizes= await _context.Sizes.ToListAsync();
            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();
            productVM.ProductImages = existed.ProductImages;
          
            

           
       
            if (!ModelState.IsValid)
            {
                return View(productVM);
            }
            if(productVM.MainPhoto is not null){
                if (!productVM.MainPhoto.CheckType("image/"))
                {
                    ModelState.AddModelError(nameof(productVM.MainPhoto), "Wrong Type!");
                    return View(productVM);
                }
                if (!productVM.MainPhoto.CheckFileSize(FileSize.MB, 2))
                {

                    ModelState.AddModelError(nameof(productVM.MainPhoto), "Wrong Size!");
                    return View(productVM);
                }

            }

            if(productVM.HoverPhoto is not null)
            {
                if (!productVM.HoverPhoto.CheckType("image/"))
                {
                    ModelState.AddModelError(nameof(productVM.HoverPhoto), "Wrong Type!");
                    return View(productVM);
                }
                if (!productVM.HoverPhoto.CheckFileSize(FileSize.MB, 2))
                {

                    ModelState.AddModelError(nameof(productVM.HoverPhoto), "Wrong Size!");
                    return View(productVM);
                }

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


            if (productVM.MainPhoto is not null)
            {
                string file = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, root);

                ProductImage main = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
                main.Image.DeleteFile(_env.WebRootPath, root);
                existed.ProductImages.Remove(main);

                existed.ProductImages.Add(new ProductImage
                {
                    IsPrimary = true,
                    IsDeleted = false,
                    DateTime = DateTime.Now,
                    Image = file


                });
                }
            if (productVM.HoverPhoto is not null)
            {
                string file = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, root);

                ProductImage hover = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == false);
                hover.Image.DeleteFile(_env.WebRootPath, root);
                existed.ProductImages.Remove(hover);

                existed.ProductImages.Add(new ProductImage
                {
                    IsPrimary = false,
                    IsDeleted = false,
                    DateTime = DateTime.Now,
                    Image = file


                });
            }
          
            if(productVM.ImageIds is null)
            {
                productVM.ImageIds = new List<int>();
            }
            var deletedfiles = existed.ProductImages.Where(pi => !productVM.ImageIds.Exists(i => i == pi.Id) && pi.IsPrimary==null);
            foreach (var file in deletedfiles)
            {
                file.Image.DeleteFile(_env.WebRootPath, root);
            }
            _context.ProductImages.RemoveRange(deletedfiles);

            if (productVM.AdditionalPhotos is not null)
            {
                string text = string.Empty;

                foreach (IFormFile file in productVM.AdditionalPhotos)
                {
                    if (!file.CheckType("image/"))
                    {
                        text += $"<div class=\"alert alert-warning\" role=\"alert\">\r\n {file.FileName} type is not correct!!\r\n</div>";
                        continue;
                    }
                    if (!file.CheckFileSize(FileSize.MB, 2))
                    {
                        text += $"<div class=\"alert alert-warning\" role=\"alert\">\r\n {file.FileName} size is not correct!!\r\n</div>";
                        continue;
                    }

                    existed.ProductImages.Add(new ProductImage
                    {
                        Image = await file.CreateFileAsync(_env.WebRootPath, root),
                        IsDeleted = false,
                        DateTime = DateTime.Now,
                        IsPrimary = null

                    });
                }
                TempData["FileWarning"] = text;
            }


            existed.SKU = productVM.SKU;
            existed.Name = productVM.Name;
            existed.Description = productVM.Description;
            existed.Price = productVM.Price.Value;
            existed.CategoryId = productVM.CategoryId.Value;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id < 1) return BadRequest();
            Product product=await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
           

            if(product is null) return NotFound();  

            product.IsDeleted= true;
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Product product = await _context.Products
            .Include(p => p.ProductImages)
            .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
            .Include(p => p.ProductSizes).ThenInclude(pc => pc.Size)
            .Include(p => p.Tags).ThenInclude(t => t.Tag)
            .Include(P => P.Category).FirstOrDefaultAsync(p=> p.Id == id);

             DetailProductVM productVM = new()
            { 
              Name = product.Name,
              Price = product.Price,
              SKU = product.SKU,
              CategoryName= product.Category.Name,
              Tags= product.Tags,
              Colors= product.ProductColors,
              Sizes= product.ProductSizes,
              Image=product.ProductImages.FirstOrDefault(i=>i.IsPrimary==true).Image
             

            };
            
            return View(productVM);
            
        }
        
    }
}
