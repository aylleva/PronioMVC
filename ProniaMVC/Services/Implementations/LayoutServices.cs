using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProniaMVC.DAL;
using ProniaMVC.Models;
using ProniaMVC.Services.Interfaces;
using ProniaMVC.ViewModels;

namespace ProniaMVC.Services.Implementations
{
    public class LayoutServices : ILayoutServices
    {
        private readonly AppDBContext context;
        private readonly IHttpContextAccessor http;

        public LayoutServices(AppDBContext context,IHttpContextAccessor http)
        {
            this.context = context;
            this.http = http;
        }

        public async Task<List<BasketItemVM>> GetBasketAsync()
        {
            List<BasketCookieItemVM> cookievm;
            string cookie = http.HttpContext.Request.Cookies["basket"];

            List<BasketItemVM> basketvm = new();
            if (cookie is null)
            {
                return basketvm;
            }

            cookievm = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookie);

            foreach (var item in cookievm)
            {
                Product? product = await context.Products
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
               return basketvm;
        }

        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
          Dictionary<string,string> settings=await context.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);
            return settings;    
        }
    }
}
