using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Newtonsoft.Json;
using ProniaMVC.DAL;
using ProniaMVC.Models;
using ProniaMVC.ViewModels;
using System.Collections.Generic;
using System.Security.Claims;

namespace ProniaMVC.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<AppUser> _usermanager;

        public BasketController(AppDBContext context,UserManager<AppUser> usermanager)
        {
            _context = context;
            _usermanager = usermanager;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> basketvm = new();
            if(User.Identity.IsAuthenticated)
            {
                basketvm=await _context.BasketItems.Where(b=>b.Userid==User.FindFirstValue(ClaimTypes.NameIdentifier))
                .Select(b=> new BasketItemVM 
                { 
                Id=b.ProductId,
                Name=b.Product.Name,
                Image=b.Product.ProductImages.FirstOrDefault(i=>i.IsPrimary==true).Image,
                Price=b.Product.Price,
                Count=b.Count,
                SubTotal=b.Count*b.Product.Price
                }
                
                ).ToListAsync();
            }
            else
            {
                List<BasketCookieItemVM> cookievm;
                string cookie = Request.Cookies["basket"];


                if (cookie is null)
                {
                    return View(basketvm);
                }

                cookievm = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookie);

                foreach (var item in cookievm)
                {
                    Product? product = await _context.Products
                    .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                    .FirstOrDefaultAsync(p => p.Id == item.Id);

                    if (product is not null)
                    {
                        basketvm.Add(new BasketItemVM
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Price = product.Price,
                            Image = product.ProductImages[0].Image,
                            Count = item.Count,
                            SubTotal = item.Count * product.Price

                        });
                    }

                }
            }
            return View(basketvm);
        }
        public async Task<IActionResult> AddBasket(int? id)
        {
            if(id is null||id<1) return BadRequest();
            bool result=await _context.Products.AnyAsync(p => p.Id == id);  
            if(!result) return NotFound();

            if(User.Identity.IsAuthenticated)
            {
                var user = await _usermanager.Users
                    .Include(u => u.BasketItems)
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

               var item= user.BasketItems.FirstOrDefault(b=>b.ProductId == id);

                if(item is null)
                {
                    user.BasketItems.Add(new BasketItems { ProductId = id.Value, Count = 1 });
                }
                else
                {
                    item.Count++;
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                List<BasketCookieItemVM> basket;
                string cookie = Request.Cookies["basket"];

                if (cookie != null)
                {
                    basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookie);
                    BasketCookieItemVM existed = basket.FirstOrDefault(b => b.Id == id);
                    if (existed != null)
                    {
                        existed.Count++;
                    }
                    else
                    {
                        basket.Add(new() { Id = id.Value, Count = 1 });

                    }

                }
                else
                {
                    basket = new();
                    basket.Add(new() { Id = id.Value, Count = 1 });
                }

                string json = JsonConvert.SerializeObject(basket);
                Response.Cookies.Append("basket", json);
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult GetBasket()
        {
            return Content(Request.Cookies["basket"]);
        }
    }
}
