using System;
using System.Threading.Tasks;
using BusinessObjects.Common;
using BusinessObjects.DTO.Appointment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Controllers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LawyerController : ControllerBase
    {
        private readonly ILawyerService _lawyerService;

        public LawyerController(ILawyerService lawyerService)
        {
            _lawyerService = lawyerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLawyers()
        {
            try
            {
                var lawyers = await _lawyerService.GetAllLawyersAsync();
                return Ok(lawyers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Status = false, Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLawyer(int id)
        {
            try
            {
                var lawyer = await _lawyerService.GetLawyerByIdAsync(id);
                if (lawyer == null)
                {
                    return NotFound(new ApiResponse { Status = false, Message = "Lawyer not found" });
                }
                
                return Ok(lawyer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Status = false, Message = ex.Message });
            }
        }
    }
}
