using BusinessObjects.Models;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.DTO.User
{
    public class CreateUserRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
        public UserRole Role { get; set; } = UserRole.Customer;
    }
}
