using Azure.Core;
using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProniaMVC.DAL;
using ProniaMVC.Models;
using ProniaMVC.Services.Interfaces;
using ProniaMVC.ViewModels;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace ProniaMVC.Services.Implementations
{
    public class BasketServices:IBasketServices
    {
        private readonly AppDBContext _context;
        private readonly IHttpContextAccessor _http;
        private readonly UserManager<AppUser> _usermanager;
        public readonly ClaimsPrincipal _user;
        public BasketServices(AppDBContext context,IHttpContextAccessor http,UserManager<AppUser> usermanager)
        {
           _context = context;
            _http = http;
           _usermanager = usermanager;
            _user = http.HttpContext.User;
        }

       

        public async Task<List<BasketItemVM>> GetBasketAsync()
        {
            List<BasketItemVM> basketvm = new();

            if (_user.Identity.IsAuthenticated)
            {
                basketvm = await _context.BasketItems.Where(b => b.Userid == _user.FindFirstValue(ClaimTypes.NameIdentifier))
                .Select(b => new BasketItemVM
                {
                    Id = b.ProductId,
                    Name = b.Product.Name,
                    Image = b.Product.ProductImages.FirstOrDefault(i => i.IsPrimary == true).Image,
                    Price = b.Product.Price,
                    Count = b.Count,
                    SubTotal = b.Count * b.Product.Price
                }

                ).ToListAsync();
            }
            else
            {
                List<BasketCookieItemVM> cookievm;
                string cookie = _http.HttpContext.Request.Cookies["basket"];


                if (cookie is null)
                {
                    return basketvm;
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
            return basketvm;
        }


      
       
    }
}
