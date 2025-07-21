using BusinessObjects.Models;

namespace BusinessObjects.DTO.User
{
    public class UpdateUserRequest
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
        public string? Password { get; set; }
    }
}
