using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProniaMVC.DAL;
using ProniaMVC.Models;
using ProniaMVC.Utilitie.Exceptions;
using ProniaMVC.ViewModels;
using System.Security.Claims;

namespace ProniaMVC.Controllers
{
    public class WishlistController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<AppUser> _usermanager;

        public WishlistController(AppDBContext context,UserManager<AppUser> usermanager)
        {
            _context = context;
            _usermanager = usermanager;
        }
        public async Task<IActionResult> Index()
        {
            List<WishlistItemVM> wishlist=new();

           

            if(User.Identity.IsAuthenticated)
            {
                wishlist = await _context.WishListItems.Where(w => w.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                .Select(w => new WishlistItemVM { 

                 
                 ProductId = w.ProductId,
                 Image=w.Product.ProductImages.FirstOrDefault(i=>i.IsPrimary==true).Image,
                 Price=w.Product.Price,
                 Name=w.Product.Name,
                 Status=w.Product.IsDeleted

                }).ToListAsync();
                
  
            }
            else
            {
                List<WishListCookieItemVm> cookievm;
                string cookie = Request.Cookies["wishlist"];

                if(cookie == null)
                {
                    return View(wishlist);
                }
                else
                {
                    cookievm=JsonConvert.DeserializeObject<List<WishListCookieItemVm>>(cookie);

                    foreach(var item in cookievm)
                    {
                        Product product=await _context.Products.Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary==true))
                       .FirstOrDefaultAsync(p=>p.Id==item.Id);

                        if(product is not null)
                        {
                            wishlist.Add(new WishlistItemVM
                            {
                                ProductId=product.Id,
                                Image = product.ProductImages[0].Image,
                                Price=product.Price,
                                Name=product.Name,
                                Status=product.IsDeleted
                            });
                        }
                    }
                }
              
            }
            return View(wishlist);

        }

        public async Task<IActionResult> Addwishlist(int? id)
        {
            if (id is null || id < 1) throw new BadRequestException("Ups ERROR");
            bool result = await _context.Products.AnyAsync(p => p.Id == id);
            if (!result) throw new NotFountException($"Ups Not Found!!");

            if (User.Identity.IsAuthenticated)
            {
                var user = await _usermanager.Users.Include(p => p.WishListItems).FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                var item = await _context.WishListItems.FirstOrDefaultAsync(w => w.ProductId == id);

                if (item is null)
                {
                    user.WishListItems.Add(new WishListItems { ProductId = id.Value });
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                List<WishListCookieItemVm> cookievm;
                string cookie = Request.Cookies["wishlist"];

                if (cookie != null)
                {
                    cookievm = JsonConvert.DeserializeObject<List<WishListCookieItemVm>>(cookie);
                    var existed = cookievm.FirstOrDefault(w => w.Id == id);

                    if (existed is null)
                    {
                        cookievm.Add(new() { Id = id.Value });
                    }
                }
                else
                {
                    cookievm = new();
                    cookievm.Add(new() { Id = id.Value });
                }
                String json = JsonConvert.SerializeObject(cookievm);
                Response.Cookies.Append("wishlist", json);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id < 1) throw new BadRequestException("Ups ERROR");
            bool result = await _context.Products.AnyAsync(p => p.Id == id);
            if (!result) throw new NotFountException($"Ups Not Found!!");

            if (User.Identity.IsAuthenticated)
            {
                var user = await _usermanager.Users.Include(u => u.WishListItems).FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                var item = await _context.WishListItems.FirstOrDefaultAsync(bi => bi.ProductId == id);

                if (item is not null)
                {
                    user.WishListItems.Remove(item);
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                List<WishListCookieItemVm> wishlist;
                string cookie = Request.Cookies["basket"];

                if (cookie != null)
                {
                    wishlist = JsonConvert.DeserializeObject<List<WishListCookieItemVm>>(cookie);
                    var existed = wishlist.FirstOrDefault(b => b.Id == id);

                    if (existed is not null)
                    {
                        wishlist.Remove(existed);
                    }

                    string json = JsonConvert.SerializeObject(wishlist);
                    Response.Cookies.Append("basket", json);

                }

            }
            return RedirectToAction("Index", "Wishlist");
        }
    }
}
