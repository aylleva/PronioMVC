namespace ProniaMVC.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string body, bool isHTML);
    }
}
