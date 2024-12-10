using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.Models;
using ProniaMVC.Utilitie.Enums;
using ProniaMVC.ViewModels;

namespace ProniaMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _usernmanager;
        private readonly SignInManager<AppUser> _signuser;
        private readonly RoleManager<IdentityRole> _userrole;

        public AccountController(UserManager<AppUser> usernmanager,SignInManager<AppUser> signuser,RoleManager<IdentityRole> userrole)
        {
            _usernmanager = usernmanager;
             _signuser = signuser;
          _userrole = userrole;
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

            if(!result.Succeeded)
            {
               foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }
            await _usernmanager.AddToRoleAsync(user,UserRole.Member.ToString());
            await _signuser.SignInAsync(user,false);
          return RedirectToAction(nameof(HomeController.Index),"Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM uservm,string? returnurl)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            AppUser user = await _usernmanager.Users.FirstOrDefaultAsync(u => u.UserName == uservm.UserorEmail || u.Email == uservm.UserorEmail);
            if(user == null)
            {
                ModelState.AddModelError(string.Empty, "Username/Email or Password is not correct!");
                return View();
            }

           var result= await _signuser.PasswordSignInAsync(user, uservm.Password, uservm.IsPersisted, true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Your acoount is blocked!Try again!");
                return View();
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Username/Email or Password is not correct!");
                return View();
            }
          
            if (returnurl is null){
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
           
            return Redirect(returnurl);


        }
        public async Task<IActionResult> Logout()
        {
            await _signuser.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }


        public async Task<IActionResult> CreateRoles()
        {
       
            foreach(var role in Enum.GetValues(typeof(UserRole)))
            {
              if(! await _userrole.RoleExistsAsync(role.ToString()))
                    {
                        await _userrole.CreateAsync(new IdentityRole { Name = role.ToString() });
                    }
                
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
