using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<bool> IsEmailExistAsync(string email);
        Task<string> GetUserRoleAsync(int userId);
    }
} 