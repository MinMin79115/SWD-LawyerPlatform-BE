using BusinessObjects.DBContext;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepository;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Implements
{
    public class ServiceRepository : GenericRepository<Service>, IServiceRepository
    {
        public ServiceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Service>> GetServicesByLawtypeAsync(string lawtypeName)
        {
            var lawtype = await _context.Lawtypes
                .FirstOrDefaultAsync(l => l.Lawtype1 == lawtypeName);
            
            if (lawtype == null)
                return new List<Service>();
                
            return await _dbSet
                .Include(s => s.Duration)
                .Where(s => s.Lawtypeid == lawtype.Lawtypeid)
                .ToListAsync();
        }
    }
}
