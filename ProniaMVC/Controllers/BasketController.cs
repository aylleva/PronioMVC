using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Newtonsoft.Json;
using NuGet.ContentModel;
using ProniaMVC.DAL;
using ProniaMVC.Models;
using ProniaMVC.Services.Interfaces;
using ProniaMVC.Utilitie.Exceptions;
using ProniaMVC.ViewModels;
using System.Collections.Generic;
using System.Security.Claims;

namespace ProniaMVC.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<AppUser> _usermanager;
        private readonly IBasketServices _basketservice;
        private readonly IEmailService _emailservice;

        public BasketController(AppDBContext context, UserManager<AppUser> usermanager, IBasketServices basketservice, IEmailService emailservice)
        {
            _context = context;
            _usermanager = usermanager;
            _basketservice = basketservice;
            _emailservice = emailservice;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> basketvm = new();
            if (User.Identity.IsAuthenticated)
            {
                basketvm = await _context.BasketItems.Where(b => b.Userid == User.FindFirstValue(ClaimTypes.NameIdentifier))
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
            if (id is null || id < 1) throw new BadRequestException("Ups ERROR");
            bool result = await _context.Products.AnyAsync(p => p.Id == id);
            if (!result) throw new NotFountException($"Ups Not Found!!");

            if (User.Identity.IsAuthenticated)
            {
                var user = await _usermanager.Users
                    .Include(u => u.BasketItems)
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                var item = user.BasketItems.FirstOrDefault(b => b.ProductId == id);

                if (item is null)
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
            return RedirectToAction(nameof(GetBasket));
        }

        public async Task<IActionResult> GetBasket()
        {
            return PartialView("BasketPartialView", await _basketservice.GetBasketAsync());
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id < 1) throw new BadRequestException("Ups ERROR");
            bool result = await _context.Products.AnyAsync(p => p.Id == id);
            if (!result) throw new NotFountException($"Ups Not Found!!");

            if (User.Identity.IsAuthenticated)
            {
                var user = await _usermanager.Users.Include(u => u.BasketItems).FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                var item = await _context.BasketItems.FirstOrDefaultAsync(bi => bi.ProductId == id);

                if (item is not null)
                {
                    user.BasketItems.Remove(item);
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
                    var existed = basket.FirstOrDefault(b => b.Id == id);

                    if (existed is not null)
                    {
                        basket.Remove(existed);
                    }

                    string json = JsonConvert.SerializeObject(basket);
                    Response.Cookies.Append("basket", json);

                }

            }
            return RedirectToAction("Index", "Basket");

        }

        public async Task<IActionResult> Pluscount(int? id)
        {
            if (id is null || id < 1) throw new BadRequestException("Ups ERROR");
            bool result = await _context.Products.AnyAsync(p => p.Id == id);
            if (!result) throw new NotFountException($"Ups Not Found!!");


            if (User.Identity.IsAuthenticated)
            {
                var user = await _usermanager.Users
               .Include(u => u.BasketItems)
               .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                var item = user.BasketItems.FirstOrDefault(b => b.ProductId == id);

                if (item is not null)
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
                    var existed = basket.FirstOrDefault(b => b.Id == id);

                    if (existed is not null)
                    {
                        existed.Count++;

                    }

                    string json = JsonConvert.SerializeObject(basket);
                    Response.Cookies.Append("basket", json);
                }
            }
            return RedirectToAction("Index", "Basket");
        }

        public async Task<IActionResult> Minuscount(int? id)
        {
            if (id is null || id < 1) throw new BadRequestException("Ups ERROR");
            bool result = await _context.Products.AnyAsync(p => p.Id == id);
            if (!result) throw new NotFountException($"Ups Not Found!!");

            if (User.Identity.IsAuthenticated)
            {
                var user = await _usermanager.Users.Include(u => u.BasketItems)
                .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                var item = await _context.BasketItems.FirstOrDefaultAsync(b => b.ProductId == id);

                if (item is not null)
                {
                    item.Count--;
                }
                if (item.Count == 0)
                {
                    _context.BasketItems.Remove(item);
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
                        existed.Count--;
                    }
                    if (existed.Count == 0)
                    {
                        basket.Remove(existed);
                    }
                    string json = JsonConvert.SerializeObject(basket);
                    Response.Cookies.Append("basket", json);
                }

            }
            return RedirectToAction("Index", "Basket");

        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Checkout()
        {

            OrderVm ordervm = new()
            {
                BasketInOrderVMs = await _context.BasketItems
                .Where(bi => bi.Userid == User.FindFirstValue(ClaimTypes.NameIdentifier))
                .Select(bi => new BasketInOrderVM
                {
                    Count = bi.Count,
                    Name = bi.Product.Name,
                    Price = bi.Product.Price,
                    SubTotal = bi.Count * bi.Product.Price


                }).ToListAsync()


            };
            return View(ordervm);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(OrderVm ordervm)
        {
            var user = await _usermanager.Users
                    .Include(u => u.BasketItems)
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

            var basketitems = await _context.BasketItems
                .Include(b => b.Product)
                .Where(b => b.Userid == User.FindFirstValue(ClaimTypes.NameIdentifier))
                .ToListAsync();

            if (!ModelState.IsValid)
            {
                ordervm.BasketInOrderVMs = basketitems.Select(bi => new BasketInOrderVM
                {
                    Count = bi.Count,
                    Name = bi.Product.Name,
                    Price = bi.Product.Price,
                    SubTotal = bi.Count * bi.Product.Price


                }).ToList();
                return View(ordervm);
            };

            Order order = new Order
            {
                Address = ordervm.Addres,
                DateTime = DateTime.Now,
                IsDeleted = false,
                Status = null,
                Userid = User.FindFirstValue(ClaimTypes.NameIdentifier),

                Orderitems = basketitems.Select(b => new OrderItem
                {
                    Userid = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    ProductId = b.Product.Id,
                    Price = b.Product.Price,
                    Count = b.Count

                }).ToList(),
                Subtotal = basketitems.Sum(b => b.Count * b.Product.Price)
            };

            _context.Orders.Add(order);
            _context.BasketItems.RemoveRange(basketitems);
            await _context.SaveChangesAsync();

            decimal total = 0;
          

            string body = @"<div class=""col-lg-6 col-12"">
	<div class=""your-order"">
		<h3>Your order</h3>
		<div class=""your-order-table table-responsive"">
			<table class=""table"">
				<thead>
					<tr>
						<th class=""cart-product-name"">Product</th>
						<th class=""cart-product-total"">Price</th>
						<th class=""cart-product-total"">Total</th>
					</tr>
				</thead>
				<tbody>";
            foreach (var item in order.Orderitems)
            {
                total +=item.Price*item.Count;
                body += @$"
						<tr class=""cart_item"">
							<td class=""cart-product-name"">
								{item.Product.Name}<strong class=""product-quantity"">
									× {item.Count}
								</strong>
							</td>
							<td class=""cart-product-total"">
								<span class=""amount"">{item.Price}</span>
							</td>
							<td class=""cart-product-total"">
								<span class=""amount"">{item.Price * item.Count}</span>
							</td>
						</tr>
					}}

				</tbody>"
                ;

              
            }
            body += @$"<tfoot>
                                    <tr class=""order-total"">
                                        <th>Order Total</th>
                                        <td>
                                            <strong><span class=""amount"">{total}</span></strong>
                                        </td>
                                    </tr>
                                </tfoot>
                              <p>Thanks For Choosing Us!</p>
";


            await _emailservice.SendEmailAsync(user.Email, "Your Order", body, true);

            return RedirectToAction(nameof(HomeController.Index), "Home");



        }

    }




}
