using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessObjects.Common;
using BusinessObjects.DTO.Appointment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Controllers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILawyerService _lawyerService;

        public AppointmentController(IAppointmentService appointmentService, ILawyerService lawyerService)
        {
            _appointmentService = appointmentService;
            _lawyerService = lawyerService;
        }

        [HttpGet("time-slots")]
        public async Task<IActionResult> GetTimeSlots([FromQuery] string date, [FromQuery] int? lawyerId)
        {
            try
            {
                DateOnly? scheduledDate = null;
                if (!string.IsNullOrEmpty(date) && DateOnly.TryParse(date, out var parsedDate))
                {
                    scheduledDate = parsedDate;
                }

                var timeSlots = await _appointmentService.GetTimeSlotsAsync(scheduledDate, lawyerId);
                return Ok(timeSlots);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Status = false, Message = ex.Message });
            }
        }

        [HttpGet("consultation-types")]
        public async Task<IActionResult> GetConsultationTypes()
        {
            try
            {
                var consultationTypes = await _appointmentService.GetConsultationTypesAsync();
                return Ok(consultationTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Status = false, Message = ex.Message });
            }
        }

        [HttpGet("duration-options")]
        public async Task<IActionResult> GetDurationOptions()
        {
            try
            {
                var durationOptions = await _appointmentService.GetDurationOptionsAsync();
                return Ok(durationOptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Status = false, Message = ex.Message });
            }
        }

        [HttpGet("consultation-methods")]
        public async Task<IActionResult> GetConsultationMethods()
        {
            try
            {
                var consultationMethods = await _appointmentService.GetConsultationMethodsAsync();
                return Ok(consultationMethods);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Status = false, Message = ex.Message });
            }
        }

        [HttpGet("calculate-price")]
        public async Task<IActionResult> CalculatePrice([FromQuery] string consultationType, [FromQuery] string duration, [FromQuery] string method)
        {
            try
            {
                if (string.IsNullOrEmpty(consultationType) || string.IsNullOrEmpty(duration) || string.IsNullOrEmpty(method))
                {
                    return BadRequest(new ApiResponse { Status = false, Message = "Vui lòng cung cấp đầy đủ thông tin để tính giá." });
                }

                var priceInfo = await _appointmentService.CalculateConsultationPriceAsync(consultationType, duration, method);
                return Ok(priceInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Status = false, Message = ex.Message });
            }
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitAppointment(AppointmentRequestDTO request)
        {
            try
            {
                // UserId đã nằm trong request, không cần truyền riêng nữa
                System.Diagnostics.Debug.WriteLine($"Using userId from request: {request.UserId}");

                var response = await _appointmentService.SubmitAppointmentAsync(request);
                
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Status = false, Message = ex.Message });
            }
        }

        // Đã bỏ Authorize
        [HttpGet("user-appointments")]
        public async Task<IActionResult> GetUserAppointments([FromQuery] int userId)
        {
            try
            {
                // Sử dụng userId được truyền từ query parameter thay vì từ token

                var appointments = await _appointmentService.GetUserAppointmentsAsync(userId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Status = false, Message = ex.Message });
            }
        }
    }
}
