using System;
using System.Threading.Tasks;
using BusinessObjects.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingController : ControllerBase
    {
        // Xoá IGoogleMeetService và inject

        /// <summary>
        /// Trả về link Google Meet dùng chung
        /// </summary>
        [HttpPost("create")]
        [Authorize]
        public IActionResult CreateMeeting()
        {
            var meetingLink = "https://meet.google.com/lookup/your-shared-link"; // Thay bằng link dùng chung của bạn
            return Ok(new ApiResponse
            {
                Code = 200,
                Status = true,
                Message = "Link Google Meet dùng chung",
                Data = new { MeetingLink = meetingLink }
            });
        }
    }
} 