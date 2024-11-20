using Microsoft.AspNetCore.Mvc;
using ProniaMVC.DAL;
using ProniaMVC.Models;
using ProniaMVC.ViewModels;

namespace ProniaMVC.Controllers
{
    public class HomeController:Controller
    {
        public readonly AppDBContext _context;
        public HomeController(AppDBContext context)
        {
            _context = context;     
        }

        public IActionResult Index()
        {
            
            List<Slide> slides = new List<Slide>
            {
                new Slide
                {Title="Flower 1",
                SubTitle="Green color",
                Description="Rare",
                Image="1-1-524x617.png",
                Order=2,
                IsDeleted=false,
                DateTime=DateTime.Now

                },

                 new Slide
                {Title="Flower 2",
                SubTitle="Red color",
                Description="Rare",
                Image="images.jpeg",
                Order=1,
                IsDeleted=false,
                DateTime=DateTime.Now

                },
                  new Slide
                {Title="Flower 3",
                SubTitle="Green color",
                Description="Rare",
                Image="1-2-524x617.png",
                Order=3,
                IsDeleted=false,
                DateTime=DateTime.Now

                }
            }
               ;

            _context.Slides.AddRange(slides);  
            _context.SaveChanges();

            HomeVM vm = new HomeVM { Slides = _context.Slides.OrderBy(s=>s.Order).Take(2).ToList()};
            return View(vm);  
        }
    }
}
