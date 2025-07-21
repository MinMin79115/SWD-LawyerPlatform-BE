using System;
using System.Threading.Tasks;
using BusinessObjects.Common;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Repositories.Interfaces;
using Microsoft.Extensions.Options;
using BusinessObjects.DTO.Meeting;

namespace Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly GoogleMeetSettings _googleMeetSettings;

        public AppointmentController(IUnitOfWork unitOfWork, IEmailService emailService, IUserService userService, IOptions<GoogleMeetSettings> googleMeetOptions)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _userService = userService;
            _googleMeetSettings = googleMeetOptions.Value;
        }

        /// <summary>
        /// Đặt lịch hẹn và gửi email xác nhận
        /// </summary>
        [HttpPost("book")]
        [Authorize]
        public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentRequest request)
        {
            // Lưu appointment vào DB
            var appointment = new Appointment
            {
                Userid = request.Userid,
                Serviceid = request.Serviceid,
                Lawyerid = request.Lawyerid,
                Scheduledate = request.Scheduledate,
                Scheduletime = request.Scheduletime,
                Meetinglink = _googleMeetSettings.FixedLink, // Lấy link từ cấu hình
                Status = AppointmentStatus.Pending,
                Createdat = DateTime.Now,
                Updatedat = DateTime.Now
            };
            await _unitOfWork.Repository<Appointment>().AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            // Lấy thông tin user, service, lawyer
            var user = await _userService.GetUserByIdAsync(appointment.Userid.Value);
            var service = await _unitOfWork.Repository<Service>().GetByIdAsync(appointment.Serviceid.Value);
            var lawyer = await _unitOfWork.Repository<Lawyer>().GetByIdAsync(appointment.Lawyerid.Value);
            var lawyerName = lawyer?.User?.Name ?? "";
            var serviceName = service?.Servicestype?.ToString() ?? "";

            // Gửi email xác nhận
            await _emailService.SendEmailWithTemplateAsync(
                user.Email,
                "Xác nhận lịch hẹn thành công",
                "AppointmentConfirmation",
                new
                {
                    UserName = user.Name,
                    Scheduledate = appointment.Scheduledate.ToString("dd/MM/yyyy"),
                    Scheduletime = appointment.Scheduletime.ToString(@"hh\:mm"),
                    ServiceName = serviceName,
                    LawyerName = lawyerName,
                    Meetinglink = appointment.Meetinglink
                }
            );

            // Map sang DTO để trả về tránh vòng lặp
            var responseDto = new AppointmentResponseDto
            {
                Appointmentid = appointment.Appointmentid,
                Userid = appointment.Userid,
                Serviceid = appointment.Serviceid,
                Lawyerid = appointment.Lawyerid,
                Scheduledate = appointment.Scheduledate,
                Scheduletime = appointment.Scheduletime,
                Meetinglink = appointment.Meetinglink,
                Status = appointment.Status.ToString(),
                Createdat = appointment.Createdat,
                Updatedat = appointment.Updatedat
            };

            return Ok(new ApiResponse
            {
                Code = 200,
                Status = true,
                Message = "Đặt lịch thành công, đã gửi email xác nhận!",
                Data = responseDto
            });
        }
    }
} 