using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.DAL;
using ProniaMVC.Models;
using ProniaMVC.Utilitie.Enums;
using ProniaMVC.Utilitie.Exceptions;
using ProniaMVC.ViewModels;

namespace ProniaMVC.Controllers
{
    public class ShopController:Controller

    {
        private readonly AppDBContext _context;

        public ShopController(AppDBContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string? search,int? categoryId,int key=1,int page=1)
        {
            IQueryable<Product> query = _context.Products.Include(p => p.ProductImages.Where(i=>i.IsPrimary!=null));

            if(!string.IsNullOrEmpty(search) )
            {
                query=query.Where(q=>q.Name.ToLower().Contains(search.ToLower()));
            }

            if(categoryId!=null && categoryId > 0)
            {
                query=query.Where(q=>q.CategoryId==categoryId);
            }

            switch (key) { 
            
             case (int)SortType.Name:
                    query=query.OrderBy(q=>q.Name); break;
              case (int)SortType.Price:
                    query = query.OrderByDescending(q => q.Price); break;
              case (int)SortType.Time:
                    query = query.OrderByDescending(q => q.DateTime); break;
            }

            int count=query.Count();
            double total = Math.Ceiling((double)count / 2);

            query = query.Skip((page - 1) * 2).Take(2);
            ShopVM shopvm = new ShopVM
            {
                Products = await query.Select(p => new GetProductVM {

                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Image = p.ProductImages.FirstOrDefault(i => i.IsPrimary == true).Image,
                    SecondImage = p.ProductImages.FirstOrDefault(i => i.IsPrimary == false).Image,
                    Description=p.Description
                }).ToListAsync(),

                Categories = await _context.Categories.Include(c => c.Product).Select(c => new GetCategoryVM {

                    Id = c.Id,
                    Name = c.Name,
                    Count = c.Product.Count
                }).ToListAsync(),

                Search = search,
                CategoryId = categoryId,
                Key = key,
                TotalPage = total,
                CurrectPage = page,
            };
           return View(shopvm);
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null || id < 1) throw new BadRequestException("Ups ERROR");

            Product? product =await _context.Products
                .Include(p=>p.ProductImages.OrderByDescending(p=>p.IsPrimary))
                .Include(p=>p.Category)
                .Include(p=>p.Tags)
                .ThenInclude(t=>t.Tag)
                .Include(p=>p.ProductColors)
                .ThenInclude(pc=>pc.Color)
                .Include(p=>p.ProductSizes)
                .ThenInclude(ps=>ps.Size)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) throw new NotFountException($"Ups Not Found!!");

            DetailVM detailVM = new DetailVM
            {
                Product = product,
                RelatedProducts =await _context.Products
                .Where(p => p.CategoryId == product.CategoryId && p.Id != id)
                .Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary!=null)) 
                .Take(8)
                .ToListAsync()
            };

            return View(detailVM);
        }
    }
}
