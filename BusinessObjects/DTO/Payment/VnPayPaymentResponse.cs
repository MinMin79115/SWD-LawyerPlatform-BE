namespace BusinessObjects.DTO.Payment
{
    public class VnPayPaymentResponse
    {
        public string PaymentUrl { get; set; } = string.Empty;
        public string PaymentId { get; set; } = string.Empty;
        public string TransactionRef { get; set; } = string.Empty;
    }
}
