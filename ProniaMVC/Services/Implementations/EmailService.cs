using ProniaMVC.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace ProniaMVC.Services.Implementations
{
    public class EmailService:IEmailService
    {
        private readonly IConfiguration _configurations;

        public EmailService(IConfiguration configurations)
        {
            _configurations = configurations;
        }

        public async Task SendEmailAsync(string email,string subject,string body,bool isHTML)
        {
           SmtpClient smtp = new SmtpClient(_configurations["Email:Host"], Convert.ToInt32(_configurations["Email:Post"]));
           smtp.EnableSsl = true;
           smtp.Credentials=new NetworkCredential(_configurations["Email:LoginEmail"],_configurations["Email:Password"]);

            MailAddress from = new MailAddress(_configurations["Email:LoginEmail"], "PRONIA");
            MailAddress to = new MailAddress(email);

            MailMessage message = new MailMessage(from,to);
            message.Subject = subject;
            message.IsBodyHtml= isHTML;
            message.Body = body;

            await smtp.SendMailAsync(message);
        }
    }
}
