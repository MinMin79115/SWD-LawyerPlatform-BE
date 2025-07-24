namespace BusinessObjects.DTO.Email
{
    public class EmailMessageDto
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string? AttachmentPath { get; set; }
    }
} 