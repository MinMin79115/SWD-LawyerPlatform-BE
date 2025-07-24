using System.Threading.Tasks;
using BusinessObjects.DTO.Email;

namespace Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessageDto emailMessage);
        Task SendEmailWithTemplateAsync(string to, string subject, string templateName, object model);
    }
} 