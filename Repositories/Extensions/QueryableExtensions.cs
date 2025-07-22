using System;
using System.Linq;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<IQueryable<Lawtype>> GetLawtypesWithCaseInsensitiveColumns(this IQueryable<Lawtype> query)
        {
            // This is a workaround for PostgreSQL case sensitivity
            // It essentially forces EF Core to use the column names exactly as defined in the database
            var lawtypes = await query.ToListAsync();
            return lawtypes.AsQueryable();
        }
    }
}
