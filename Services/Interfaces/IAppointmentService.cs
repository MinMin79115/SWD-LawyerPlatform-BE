using BusinessObjects.Common;
using BusinessObjects.DTO.Appointment;

namespace Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<ApiResponse> CreateAppointmentAsync(int userId, CreateAppointmentRequest request);
        Task<ApiResponse> GetAppointmentByIdAsync(int appointmentId);
        Task<ApiResponse> GetAppointmentsByUserIdAsync(int userId);
        Task<ApiResponse> GetAppointmentsByLawyerIdAsync(int lawyerId);
        Task<ApiResponse> GetAllAppointmentsAsync();
        Task<ApiResponse> UpdateAppointmentStatusAsync(int appointmentId, UpdateAppointmentStatusRequest request);
        Task<ApiResponse> GetLawyersAsync();
        Task<ApiResponse> GetLawtypesAsync();
        Task<ApiResponse> ConfirmPaymentAsync(int appointmentId);
    }
}
