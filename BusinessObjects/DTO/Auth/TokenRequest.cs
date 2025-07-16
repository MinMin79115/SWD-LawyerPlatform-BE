using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.DTO.Auth
{
    public class TokenRequest
    {
        [Required]
        public string Token { get; set; } = null!;

        [Required]
        public string RefreshToken { get; set; } = null!;
    }
} 