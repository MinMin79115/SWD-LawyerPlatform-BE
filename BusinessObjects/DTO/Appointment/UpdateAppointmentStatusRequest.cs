using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.DTO.Appointment
{
    public class UpdateAppointmentStatusRequest
    {
        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        [RegularExpression("^(Pending|Confirmed|Completed|Cancelled)$", 
            ErrorMessage = "Trạng thái phải là: Pending, Confirmed, Completed, hoặc Cancelled")]
        public string Status { get; set; } = null!;
    }
}
