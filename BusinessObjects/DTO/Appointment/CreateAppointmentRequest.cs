using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.DTO.Appointment
{
    public class CreateAppointmentRequest
    {
        [Required(ErrorMessage = "Vui lòng chọn loại pháp lý")]
        public int LawtypeId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn luật sư")]
        public int LawyerId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày hẹn")]
        public DateOnly ScheduleDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giờ hẹn")]
        public TimeOnly ScheduleTime { get; set; }

        [Required(ErrorMessage = "Số tiền là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
        public decimal TotalAmount { get; set; }
    }
}
