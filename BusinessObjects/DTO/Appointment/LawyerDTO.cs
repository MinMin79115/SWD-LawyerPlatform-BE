namespace BusinessObjects.DTO.Appointment
{
    public class LawyerDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
        public string Experience { get; set; } = string.Empty;
        public string[] Specialties { get; set; } = Array.Empty<string>();
        public decimal Rating { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Qualification { get; set; }
    }
}
