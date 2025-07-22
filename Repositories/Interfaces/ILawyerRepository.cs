using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface ILawyerRepository : IGenericRepository<Lawyer>
    {
        Task<IEnumerable<Lawyer>> GetLawyersWithUserInfoAsync();
        Task<Lawyer> GetLawyerByUserIdAsync(int userId);
    }
}
