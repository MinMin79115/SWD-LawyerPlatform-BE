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
            var user = await _dbSet.Where(u => u.Userid == userId).Select(u => u.Role).FirstOrDefaultAsync();
            
            if (user == null)
            {
                return "Unknown"; // Hoặc xử lý lỗi phù hợp nếu người dùng không tồn tại
            }
            
            return user; // Trả về trực tiếp string, không cần ToString()
        }
    }
} 