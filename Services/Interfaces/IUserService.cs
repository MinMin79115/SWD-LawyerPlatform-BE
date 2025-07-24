using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessObjects.Models;
using BusinessObjects.DTO.Auth;
using BusinessObjects.DTO.User;

namespace Services.Interfaces
{
    public interface IUserService
    {
        // Existing methods
        Task<User> RegisterUserAsync(RegisterRequest request);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(int userId);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> CheckPasswordAsync(string email, string password);
        
        // New CRUD methods
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> CreateUserAsync(CreateUserRequest request);
        Task<UserDto> GetUserDtoByIdAsync(int userId);
        Task<bool> UpdateUserAsync(int userId, UpdateUserRequest request);
        Task<bool> DeleteUserAsync(int userId);
        Task<string> GetUserRoleAsync(int userId);
    }
} 