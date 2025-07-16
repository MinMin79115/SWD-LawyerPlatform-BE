using System.Threading.Tasks;
using BusinessObjects.Models;
using BusinessObjects.DTO.Auth;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(RegisterRequest request);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(int userId);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> CheckPasswordAsync(string email, string password);
    }
} 