using BusinessObjects.DBContext;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepository;
using Repositories.Interfaces;

namespace Repositories.Implements
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Payment?> GetPaymentByAppointmentIdAsync(int appointmentId)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Lawyer)
                        .ThenInclude(l => l.User)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Lawtype)
                .FirstOrDefaultAsync(p => p.Appointmentid == appointmentId);
        }

        public async Task<Payment?> GetPaymentByTransactionIdAsync(string transactionId)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Appointment)
                .FirstOrDefaultAsync(p => p.Transactionid == transactionId);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(int userId)
        {
            return await _context.Payments
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Lawyer)
                        .ThenInclude(l => l.User)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Lawtype)
                .Where(p => p.Userid == userId)
                .OrderByDescending(p => p.Createdat)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(string status)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Appointment)
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.Createdat)
                .ToListAsync();
        }
    }
}
