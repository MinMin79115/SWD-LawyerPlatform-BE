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
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(a => a.Service)
                .Include(a => a.Lawyer)
                .ThenInclude(l => l!.User)
                .Where(a => a.Userid == userId)
                .OrderByDescending(a => a.Scheduledate)
                .ThenBy(a => a.Scheduletime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByLawyerIdAsync(int lawyerId)
        {
            return await _dbSet
                .Include(a => a.Service)
                .Include(a => a.User)
                .Where(a => a.Lawyerid == lawyerId)
                .OrderByDescending(a => a.Scheduledate)
                .ThenBy(a => a.Scheduletime)
                .ToListAsync();
        }

        public async Task<bool> IsTimeSlotAvailableAsync(TimeOnly time, DateOnly date, int? lawyerId)
        {
            var query = _dbSet.Where(a => a.Scheduledate == date && a.Scheduletime == time);
            
            if (lawyerId.HasValue)
            {
                query = query.Where(a => a.Lawyerid == lawyerId);
            }
            
            // If no appointment exists at that time slot, it's available
            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<TimeOnly>> GetAvailableTimeSlotsAsync(DateOnly date, int? lawyerId)
        {
            // Define all possible time slots (e.g., every hour from 8 AM to 6 PM)
            var allTimeSlots = new List<TimeOnly>();
            for (int hour = 8; hour <= 18; hour++)
            {
                allTimeSlots.Add(new TimeOnly(hour, 0, 0));
            }

            // Get booked time slots for the specified date and lawyer
            var query = _dbSet.Where(a => a.Scheduledate == date);
            
            if (lawyerId.HasValue)
            {
                query = query.Where(a => a.Lawyerid == lawyerId);
            }
            
            var bookedTimeSlots = await query.Select(a => a.Scheduletime).ToListAsync();

            // Return time slots that are not booked
            return allTimeSlots.Where(ts => !bookedTimeSlots.Contains(ts));
        }
    }
}
