using System.Linq;
using System.Threading.Tasks;
using BusinessObjects.DBContext;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepository;
using Repositories.Interfaces;

namespace Repositories.Implements
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> IsEmailExistAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<string> GetUserRoleAsync(int userId)
        {
            var user = await _dbSet.FindAsync(userId);
            if (user == null)
                return "Customer";
            
            // Chuyển đổi từ enum sang string
            return user.Role.ToString();
        }
    }
} 