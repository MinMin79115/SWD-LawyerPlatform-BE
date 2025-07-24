using BusinessObjects.DBContext;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepository;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Implements
{
    public class LawyerRepository : GenericRepository<Lawyer>, ILawyerRepository
    {
        public LawyerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Lawyer>> GetLawyersWithUserInfoAsync()
        {
            return await _dbSet
                .Include(l => l.User)
                .ToListAsync();
        }

        public async Task<Lawyer> GetLawyerByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Userid == userId);
        }

        public async Task<List<Lawyer>> GetLawyersWithDetailsAsync()
        {
            return await _dbSet
                .Include(l => l.User)
                .Where(l => l.User != null)
                .ToListAsync();
        }
    }
}
