using BusinessObjects.DBContext;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepository;
using Repositories.Interfaces;

namespace Repositories.Implements
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
        {
            appointment.Createdat = DateTime.Now;
            appointment.Updatedat = DateTime.Now;
            await AddAsync(appointment);
            return appointment;
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int appointmentId)
        {
            return await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Lawyer)
                    .ThenInclude(l => l.User)
                .Include(a => a.Lawtype)
                .Include(a => a.Payment)
                .FirstOrDefaultAsync(a => a.Appointmentid == appointmentId);
        }

        public async Task<List<Appointment>> GetAppointmentsByUserIdAsync(int userId)
        {
            return await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Lawyer)
                    .ThenInclude(l => l.User)
                .Include(a => a.Lawtype)
                .Include(a => a.Payment)
                .Where(a => a.Userid == userId)
                .OrderByDescending(a => a.Createdat)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsByLawyerIdAsync(int lawyerId)
        {
            return await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Lawyer)
                    .ThenInclude(l => l.User)
                .Include(a => a.Lawtype)
                .Include(a => a.Payment)
                .Where(a => a.Lawyerid == lawyerId)
                .OrderByDescending(a => a.Createdat)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Lawyer)
                    .ThenInclude(l => l.User)
                .Include(a => a.Lawtype)
                .Include(a => a.Payment)
                .OrderByDescending(a => a.Createdat)
                .ToListAsync();
        }

        public async Task<Appointment> UpdateAppointmentAsync(Appointment appointment)
        {
            appointment.Updatedat = DateTime.Now;
            _context.Appointments.Update(appointment);
            return appointment;
        }

        public async Task<bool> DeleteAppointmentAsync(int appointmentId)
        {
            var appointment = await GetByIdAsync(appointmentId);
            if (appointment == null) return false;

            await DeleteAsync(appointment);
            return true;
        }

        public async Task<List<Appointment>> GetAppointmentsByStatusAsync(string status)
        {
            return await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Lawyer)
                    .ThenInclude(l => l.User)
                .Include(a => a.Lawtype)
                .Include(a => a.Payment)
                .Where(a => a.Status == status)
                .OrderByDescending(a => a.Createdat)
                .ToListAsync();
        }
    }
}
