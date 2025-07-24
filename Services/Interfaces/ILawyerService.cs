using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessObjects.DTO.User;

namespace Services.Interfaces
{
    public interface ILawyerService
    {
        Task<IEnumerable<LawyerDto>> GetAllLawyersAsync();
        Task<LawyerDto> GetLawyerByIdAsync(int lawyerId);
    }
}
