using Microsoft.EntityFrameworkCore;
using ProniaMVC.DAL;
using ProniaMVC.Services.Interfaces;

namespace ProniaMVC.Services.Implementations
{
    public class LayoutServices : ILayoutServices
    {
        private readonly AppDBContext context;

        public LayoutServices(AppDBContext context)
        {
            this.context = context;
        }
        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
          Dictionary<string,string> settings=await context.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);
            return settings;    
        }
    }
}
