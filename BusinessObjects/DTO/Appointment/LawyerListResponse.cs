namespace BusinessObjects.DTO.Appointment
{
    public class LawyerListResponse
    {
        public int LawyerId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int? Experience { get; set; }
        public string? Description { get; set; }
        public string? Qualification { get; set; }
        public List<string>? Specialties { get; set; }
        public decimal? Rating { get; set; }
        public decimal? ConsultationFee { get; set; }
        public string? Avatar { get; set; }
    }
}
