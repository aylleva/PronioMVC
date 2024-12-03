using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProniaMVC.Models;
using ProniaMVC.ViewModels;

namespace ProniaMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _usernmanager;
        private readonly SignInManager<AppUser> _signuser;

        public AccountController(UserManager<AppUser> usernmanager,SignInManager<AppUser> signuser)
        {
            _usernmanager = usernmanager;
             _signuser = signuser;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM uservm)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            AppUser user = new()

            {
                Name = uservm.Name,
                Email = uservm.Email,
                Surname = uservm.Surname,
                UserName = uservm.UserName,
            };
            var result = await _usernmanager.CreateAsync(user, uservm.Password);

            if(result.Succeeded)
            {
               foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }

            await _signuser.SignInAsync(user,false);
          return RedirectToAction(nameof(HomeController.Index),"Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signuser.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
