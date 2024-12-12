using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Newtonsoft.Json;
using ProniaMVC.DAL;
using ProniaMVC.Models;
using ProniaMVC.ViewModels;
using System.Collections.Generic;

namespace ProniaMVC.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDBContext _context;

        public BasketController(AppDBContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketCookieItemVM> cookievm;
            string cookie = Request.Cookies["basket"];

            List<BasketItemVM> basketvm = new();
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

                if(product is not null)
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
            return View(basketvm);
        }
        public async Task<IActionResult> AddBasket(int? id)
        {
            if(id is null||id<1) return BadRequest();
            bool result=await _context.Products.AnyAsync(p => p.Id == id);  
            if(!result) return NotFound();

            List<BasketCookieItemVM> basket;
            string cookie = Request.Cookies["basket"];

            if(cookie != null)
            {
               basket=JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookie);
              BasketCookieItemVM existed=basket.FirstOrDefault(b=>b.Id == id);
                if(existed != null)
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

            string json=JsonConvert.SerializeObject(basket);
            Response.Cookies.Append("basket",json);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult GetBasket()
        {
            return Content(Request.Cookies["basket"]);
        }
    }
}
