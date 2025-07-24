using BusinessObjects.Common;
using BusinessObjects.DTO.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Controllers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;

        public PaymentController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        /// <summary>
        /// Tạo URL thanh toán VNPay cho appointment
        /// </summary>
        [HttpPost("create-payment-url")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreatePaymentUrl([FromBody] CreateVnPayPaymentRequest request)
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

            var result = await _vnPayService.CreatePaymentUrlAsync(request, HttpContext);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Xử lý kết quả trả về từ VNPay (Return URL)
        /// </summary>
        [HttpGet("vnpay-return")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> VnPayReturn()
        {
            var rawQueryString = Request.QueryString.Value;
            var result = await _vnPayService.ProcessReturnAsync(Request.Query, rawQueryString);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Xử lý IPN từ VNPay (Instant Payment Notification)
        /// </summary>
        [HttpGet("vnpay-ipn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> VnPayIpn()
        {
            var result = await _vnPayService.ProcessIpnAsync(Request.Query);
            
            // Trả về JSON cho VNPay
            Response.ContentType = "application/json";
            return Ok(result);
        }
    }
}
