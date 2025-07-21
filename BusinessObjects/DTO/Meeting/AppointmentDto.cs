namespace BusinessObjects.DTO.Meeting
{
    public class BookAppointmentRequest
    {
        public int Userid { get; set; }
        public int Serviceid { get; set; }
        public int Lawyerid { get; set; }
        public DateOnly Scheduledate { get; set; }
        public TimeOnly Scheduletime { get; set; }
    }

    public class AppointmentResponseDto
    {
        public int Appointmentid { get; set; }
        public int? Userid { get; set; }
        public int? Serviceid { get; set; }
        public int? Lawyerid { get; set; }
        public DateOnly Scheduledate { get; set; }
        public TimeOnly Scheduletime { get; set; }
        public string? Meetinglink { get; set; }
        public string? Status { get; set; }
        public DateTime? Createdat { get; set; }
        public DateTime? Updatedat { get; set; }
    }
} 