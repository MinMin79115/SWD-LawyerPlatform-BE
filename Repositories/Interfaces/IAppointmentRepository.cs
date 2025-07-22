using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetAppointmentsByUserIdAsync(int userId);
        Task<IEnumerable<Appointment>> GetAppointmentsByLawyerIdAsync(int lawyerId);
        Task<bool> IsTimeSlotAvailableAsync(TimeOnly time, DateOnly date, int? lawyerId);
        Task<IEnumerable<TimeOnly>> GetAvailableTimeSlotsAsync(DateOnly date, int? lawyerId);
    }
}
