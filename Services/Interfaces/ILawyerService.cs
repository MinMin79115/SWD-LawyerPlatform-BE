using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessObjects.DTO.Appointment;
using BusinessObjects.Models;

namespace Services.Interfaces
{
    public interface ILawyerService
    {
        Task<IEnumerable<LawyerDTO>> GetAllLawyersAsync();
        Task<LawyerDTO> GetLawyerByIdAsync(int lawyerId);
    }
}
