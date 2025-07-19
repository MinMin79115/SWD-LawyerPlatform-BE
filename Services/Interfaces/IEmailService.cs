using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage emailMessage);
        Task SendEmailWithTemplateAsync(string to, string subject, string templateName, object model);
    }
} 