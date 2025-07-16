using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BusinessObjects.Common;
using BusinessObjects.Models;
using Microsoft.Extensions.Options;
using Services.Interfaces;

namespace Services.Implements
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        
        public SmtpEmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(EmailMessage emailMessage)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                Subject = emailMessage.Subject,
                Body = emailMessage.Body,
                IsBodyHtml = emailMessage.IsHtml
            };

            message.To.Add(new MailAddress(emailMessage.To));

            // Thêm các tập tin đính kèm (nếu có)
            if (emailMessage.Attachments != null)
            {
                foreach (var attachment in emailMessage.Attachments)
                {
                    var memoryStream = new MemoryStream(attachment.Content);
                    message.Attachments.Add(new Attachment(memoryStream, attachment.FileName, attachment.ContentType));
                }
            }

            using (var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                client.EnableSsl = _smtpSettings.EnableSsl;

                await client.SendMailAsync(message);
            }
        }

        public async Task SendEmailWithTemplateAsync(string to, string subject, string templateName, object model)
        {
            // Đọc template từ file và thay thế các placeholder
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates", $"{templateName}.html");
            
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Email template '{templateName}' not found.");
            }

            string body = await File.ReadAllTextAsync(templatePath);

            // Thay thế các placeholder bằng dữ liệu từ model
            // Đây là cách đơn giản, trong thực tế bạn có thể sử dụng thư viện như Razor hoặc Handlebars
            foreach (var prop in model.GetType().GetProperties())
            {
                var value = prop.GetValue(model)?.ToString() ?? string.Empty;
                body = body.Replace($"{{{{{prop.Name}}}}}", value);
            }

            // Gửi email với nội dung đã tạo
            await SendEmailAsync(new EmailMessage 
            { 
                To = to, 
                Subject = subject, 
                Body = body, 
                IsHtml = true 
            });
        }
    }
} 