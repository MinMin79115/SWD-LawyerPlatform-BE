using BusinessObjects.DTO.Appointment;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<TimeSlotDTO>> GetTimeSlotsAsync(DateOnly? date = null, int? lawyerId = null);
        Task<IEnumerable<ConsultationTypeDTO>> GetConsultationTypesAsync();
        Task<IEnumerable<DurationOptionDTO>> GetDurationOptionsAsync();
        Task<IEnumerable<ConsultationMethodDTO>> GetConsultationMethodsAsync();
        Task<PriceInfoDTO> CalculateConsultationPriceAsync(string consultationType, string duration, string method);
        Task<AppointmentResponseDTO> SubmitAppointmentAsync(AppointmentRequestDTO request, int userId);
        Task<IEnumerable<AppointmentDetailsDTO>> GetUserAppointmentsAsync(int userId);
    }
}
