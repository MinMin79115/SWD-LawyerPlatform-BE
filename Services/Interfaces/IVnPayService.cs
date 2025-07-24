using BusinessObjects.Common;
using BusinessObjects.DTO.Payment;
using Microsoft.AspNetCore.Http;

namespace Services.Interfaces
{
    public interface IVnPayService
    {
        /// <summary>
        /// Tạo URL thanh toán VNPay
        /// </summary>
        Task<ApiResponse> CreatePaymentUrlAsync(CreateVnPayPaymentRequest request, HttpContext context);
        
        /// <summary>
        /// Xử lý kết quả trả về từ VNPay (Return URL)
        /// </summary>
        Task<ApiResponse> ProcessReturnAsync(IQueryCollection vnpayData, string rawQueryString = null);
        
        /// <summary>
        /// Xử lý IPN từ VNPay (cập nhật database)
        /// </summary>
        Task<string> ProcessIpnAsync(IQueryCollection vnpayData);
        
        /// <summary>
        /// Kiểm tra chữ ký từ VNPay
        /// </summary>
        bool ValidateSignature(IQueryCollection vnpayData, string secretKey);
    }
}
