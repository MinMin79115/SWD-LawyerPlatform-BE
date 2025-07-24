using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessObjects.Common;
using BusinessObjects.DTO.Appointment;
using Services.Interfaces;

namespace Controllers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        /// <summary>
        /// Lấy danh sách luật sư để booking
        /// </summary>
        [HttpGet("lawyers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLawyers()
        {
            var result = await _appointmentService.GetLawyersAsync();
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Lấy danh sách loại pháp lý để booking
        /// </summary>
        [HttpGet("lawtypes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLawtypes()
        {
            var result = await _appointmentService.GetLawtypesAsync();
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Tạo appointment mới (Customer only)
        /// </summary>
        [HttpPost]
        [Authorize] // Tạm thời bỏ role check để test
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Code = 400,
                    Status = false,
                    Message = "Dữ liệu không hợp lệ",
                    Data = ModelState
                });
            }

            // Lấy tất cả nameidentifier claims và tìm cái nào là số
            var nameIdentifierClaims = User.FindAll("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            string userIdClaim = null;
            
            // Debug: Log tất cả claims
            Console.WriteLine($"All claims:");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"  {claim.Type}: {claim.Value}");
            }
            
            // Tìm claim nameidentifier có giá trị là số
            foreach (var claim in nameIdentifierClaims)
            {
                if (int.TryParse(claim.Value, out _))
                {
                    userIdClaim = claim.Value;
                    break;
                }
            }
            
            Console.WriteLine($"UserIdClaim: {userIdClaim}");
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new ApiResponse
                {
                    Code = 401,
                    Status = false,
                    Message = $"Token không hợp lệ - UserIdClaim: {userIdClaim}"
                });
            }

            var result = await _appointmentService.CreateAppointmentAsync(userId, request);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Lấy appointment theo ID
        /// </summary>
        [HttpGet("{appointmentId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAppointment(int appointmentId)
        {
            var result = await _appointmentService.GetAppointmentByIdAsync(appointmentId);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Lấy danh sách appointment của customer hiện tại
        /// </summary>
        [HttpGet("my-appointments")]
        [Authorize] // Tạm thời bỏ role check để test
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyAppointments()
        {
            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new ApiResponse
                {
                    Code = 401,
                    Status = false,
                    Message = "Token không hợp lệ"
                });
            }

            var result = await _appointmentService.GetAppointmentsByUserIdAsync(userId);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Lấy danh sách appointment của lawyer hiện tại
        /// </summary>
        [HttpGet("lawyer-appointments")]
        [Authorize(Roles = "Lawyer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLawyerAppointments()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new ApiResponse
                {
                    Code = 401,
                    Status = false,
                    Message = "Token không hợp lệ"
                });
            }

            // Tìm lawyerId từ userId
            // Cần thêm logic để lấy lawyerId từ userId
            // Tạm thời return lỗi, sẽ implement sau
            return BadRequest(new ApiResponse
            {
                Code = 400,
                Status = false,
                Message = "Chức năng đang được phát triển"
            });
        }

        /// <summary>
        /// Lấy tất cả appointment (Admin only)
        /// </summary>
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAppointments()
        {
            var result = await _appointmentService.GetAllAppointmentsAsync();
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Cập nhật trạng thái appointment
        /// Lawyer có thể đánh dấu "Completed"
        /// Admin có thể đánh dấu "Cancelled"
        /// </summary>
        [HttpPatch("{appointmentId}/status")]
        [Authorize(Roles = "Lawyer,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAppointmentStatus(int appointmentId, [FromBody] UpdateAppointmentStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Code = 400,
                    Status = false,
                    Message = "Dữ liệu không hợp lệ",
                    Data = ModelState
                });
            }

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Kiểm tra quyền theo role
            if (userRole == "Lawyer" && request.Status != "Completed")
            {
                return Forbid("Luật sư chỉ có thể đánh dấu cuộc hẹn là 'Completed'");
            }

            if (userRole == "Admin" && request.Status != "Cancelled")
            {
                return Forbid("Admin chỉ có thể đánh dấu cuộc hẹn là 'Cancelled'");
            }

            var result = await _appointmentService.UpdateAppointmentStatusAsync(appointmentId, request);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Xác nhận thanh toán và gửi email (sẽ được gọi sau khi VNPay callback)
        /// </summary>
        [HttpPost("{appointmentId}/confirm-payment")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConfirmPayment(int appointmentId)
        {
            var result = await _appointmentService.ConfirmPaymentAsync(appointmentId);
            return StatusCode(result.Code, result);
        }
    }
}
