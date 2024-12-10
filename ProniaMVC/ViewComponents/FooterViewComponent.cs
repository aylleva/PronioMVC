using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.DAL;

namespace ProniaMVC.ViewComponents
{
    public class FooterViewComponent:ViewComponent
    {
        private readonly AppDBContext _context;

        public FooterViewComponent(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string,string> setting=await _context.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);
            return View(setting);
        }
    }
}
