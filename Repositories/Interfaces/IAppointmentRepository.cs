using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Appointment> CreateAppointmentAsync(Appointment appointment);
        Task<Appointment?> GetAppointmentByIdAsync(int appointmentId);
        Task<List<Appointment>> GetAppointmentsByUserIdAsync(int userId);
        Task<List<Appointment>> GetAppointmentsByLawyerIdAsync(int lawyerId);
        Task<List<Appointment>> GetAllAppointmentsAsync();
        Task<Appointment> UpdateAppointmentAsync(Appointment appointment);
        Task<bool> DeleteAppointmentAsync(int appointmentId);
        Task<List<Appointment>> GetAppointmentsByStatusAsync(string status);
    }
}
