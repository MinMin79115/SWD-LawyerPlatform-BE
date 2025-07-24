using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.DTO.Payment
{
    public class CreateVnPayPaymentRequest
    {
        [Required]
        public int AppointmentId { get; set; }
        
        [Required]
        [Range(1000, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 1,000 VND")]
        public decimal Amount { get; set; }
        
        public string? BankCode { get; set; }
        
        public string OrderInfo { get; set; } = string.Empty;
    }
}
