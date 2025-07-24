using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObjects.Models;
using BusinessObjects.DTO.User;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implements
{
    public class LawyerService : ILawyerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LawyerService> _logger;

        public LawyerService(IUnitOfWork unitOfWork, ILogger<LawyerService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<LawyerDto>> GetAllLawyersAsync()
        {
            try
            {
                var lawyers = await _unitOfWork.LawyerRepository.GetLawyersWithUserInfoAsync();
                
                return lawyers.Select(l => new LawyerDto
                {
                    Id = l.Lawyerid.ToString(),
                    Name = l.User?.Name ?? "Unknown",
                    Email = l.User?.Email ?? "Unknown",
                    Phone = l.User?.Phone,
                    Avatar = l.User?.Avatar,
                    Experience = l.Experience.HasValue ? $"{l.Experience.Value} năm kinh nghiệm" : "Chưa cập nhật",
                    Specialties = l.Specialties != null && l.Specialties.Count > 0 ? l.Specialties.ToArray() : new string[] { "Luật sư đa lĩnh vực" },
                    Rating = l.Rating ?? 5.0m,
                    Description = l.Description ?? "Luật sư chuyên nghiệp",
                    Qualification = l.Qualification
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all lawyers");
                throw;
            }
        }

        public async Task<LawyerDto> GetLawyerByIdAsync(int lawyerId)
        {
            try
            {
                var lawyer = await _unitOfWork.Repository<Lawyer>().GetByIdAsync(lawyerId);
                if (lawyer == null)
                {
                    return null;
                }
                
                var user = await _unitOfWork.Repository<User>().GetByIdAsync(lawyer.Userid);
                
                return new LawyerDto
                {
                    Id = lawyer.Lawyerid.ToString(),
                    Name = user?.Name ?? "Unknown",
                    Email = user?.Email ?? "Unknown",
                    Phone = user?.Phone,
                    Avatar = user?.Avatar,
                    Experience = lawyer.Experience.HasValue ? $"{lawyer.Experience.Value} năm kinh nghiệm" : "Chưa cập nhật",
                    Specialties = lawyer.Specialties != null && lawyer.Specialties.Count > 0 ? lawyer.Specialties.ToArray() : new string[] { "Luật sư đa lĩnh vực" },
                    Rating = lawyer.Rating ?? 5.0m,
                    Description = lawyer.Description ?? "Luật sư chuyên nghiệp",
                    Qualification = lawyer.Qualification
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lawyer by id");
                throw;
            }
        }
    }
}
