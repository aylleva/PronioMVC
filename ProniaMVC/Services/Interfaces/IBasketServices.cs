using ProniaMVC.ViewModels;

namespace ProniaMVC.Services.Interfaces
{
    public interface IBasketServices
    {
        Task<List<BasketItemVM>> GetBasketAsync();


    }
}
