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

        public async Task<bool> IsLawyerAsync(int userId)
        {
            // Kiểm tra xem user có phải là lawyer không bằng cách kiểm tra trong bảng Lawyer
            return await _context.Set<Lawyer>().AnyAsync(l => l.Userid == userId);
        }
    }
} 