using System.Threading.Tasks;
using BusinessObjects.Common;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailTestController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailTestController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        /// <summary>
        /// Gửi email test đơn giản
        /// </summary>
        /// <remarks>Dùng để kiểm tra cấu hình SMTP hoạt động chính xác</remarks>
        /// <param name="to">Địa chỉ email người nhận</param>
        /// <returns>Thông báo thành công hoặc thất bại</returns>
        [HttpPost("send-simple")]
        public async Task<IActionResult> SendSimpleEmail(string to)
        {
            try
            {
                var emailMessage = new EmailMessage
                {
                    To = to,
                    Subject = "Email Test từ Lawyer Platform",
                    Body = "<h1>Xin chào!</h1><p>Đây là email test từ API Lawyer Platform.</p>",
                    IsHtml = true
                };

                await _emailService.SendEmailAsync(emailMessage);

                return Ok(new ApiResponse
                {
                    Code = 200,
                    Status = true,
                    Message = "Email đã được gửi thành công",
                    Data = null
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Code = 400,
                    Status = false,
                    Message = "Không thể gửi email",
                    Data = ex.Message
                });
            }
        }

        /// <summary>
        /// Gửi email test với template
        /// </summary>
        /// <remarks>Dùng để kiểm tra template email hoạt động chính xác</remarks>
        /// <param name="to">Địa chỉ email người nhận</param>
        /// <param name="name">Tên người nhận (để điền vào template)</param>
        /// <returns>Thông báo thành công hoặc thất bại</returns>
        [HttpPost("send-template")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendTemplateEmail(string to, string name)
        {
            try
            {
                await _emailService.SendEmailWithTemplateAsync(
                    to,
                    "Chào mừng bạn đến với Nền tảng Luật sư",
                    "Welcome",
                    new { Name = name, Email = to }
                );

                return Ok(new ApiResponse
                {
                    Code = 200,
                    Status = true,
                    Message = "Email đã được gửi thành công với template",
                    Data = null
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Code = 400,
                    Status = false,
                    Message = "Không thể gửi email",
                    Data = ex.Message
                });
            }
        }
    }
} 