namespace ProniaMVC.Services.Interfaces
{
    public interface ILayoutServices
    {
        Task<Dictionary<string, string>> GetSettingsAsync();
    }
}
