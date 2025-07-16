using System.Threading.Tasks;
using BusinessObjects.Common;
using BusinessObjects.DTO.Auth;

namespace Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(LoginRequest request);
        Task<AuthResult> RegisterAsync(RegisterRequest request);
        Task<AuthResult> RefreshTokenAsync(TokenRequest tokenRequest);
        Task<string> GenerateJwtToken(string email, string role, int userId);
    }
} 