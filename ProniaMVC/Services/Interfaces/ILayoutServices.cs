using ProniaMVC.ViewModels;

namespace ProniaMVC.Services.Interfaces
{
    public interface ILayoutServices
    {
        Task<Dictionary<string, string>> GetSettingsAsync();
        Task<List<BasketItemVM>> GetBasketAsync();  
    }
}
