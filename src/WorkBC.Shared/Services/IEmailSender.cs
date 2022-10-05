using System.Threading.Tasks;

namespace WorkBC.Shared.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage, string textMessage);
    }
}