namespace BusinessObjects.DTO.Appointment
{
    public class AppointmentResponse
    {
        public int AppointmentId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public int LawtypeId { get; set; }
        public string LawtypeName { get; set; } = null!;
        public int LawyerId { get; set; }
        public string LawyerName { get; set; } = null!;
        public DateOnly ScheduleDate { get; set; }
        public TimeOnly ScheduleTime { get; set; }
        public string Status { get; set; } = null!;
        public string? MeetingLink { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
