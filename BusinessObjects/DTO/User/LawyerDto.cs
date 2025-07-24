namespace BusinessObjects.DTO.User
{
    public class LawyerDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
        public string Experience { get; set; }
        public string[] Specialties { get; set; }
        public decimal Rating { get; set; }
        public string? Description { get; set; }
        public string? Qualification { get; set; }
    }
} 