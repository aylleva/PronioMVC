using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ProniaMVC.Areas.Admin.ViewModels;

using ProniaMVC.DAL;
using ProniaMVC.Models;

namespace ProniaMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class HomeController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<AppUser> _usermanager;

        public HomeController(AppDBContext context,UserManager<AppUser> usermanager)
        {
            _context = context;
            _usermanager = usermanager;
        }
        public async Task<IActionResult> Index()
        {
         List<AdminOrdersVM> ordervm= await _context.Orders
                .Include(o=>o.User)
                .Select(o=> new AdminOrdersVM
                {
                    OrderId=o.Id,
                    UserName=o.User.UserName,
                    Status=o.Status,
                    Subtotal=o.Subtotal,
                    CreatedAt=DateTime.Now.ToString("MM/dd/yyyy"),
                }).ToListAsync();
            return View(ordervm);
        }
       
    }
}
