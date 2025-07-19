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
        private readonly IGoogleMeetService _googleMeetService;

        public MeetingController(IGoogleMeetService googleMeetService)
        {
            _googleMeetService = googleMeetService;
        }

        /// <summary>
        /// Tạo một cuộc họp Google Meet mới
        /// </summary>
        /// <param name="title">Tiêu đề cuộc họp</param>
        /// <param name="description">Mô tả cuộc họp</param>
        /// <param name="startTime">Thời gian bắt đầu (định dạng ISO: yyyy-MM-ddTHH:mm:ss)</param>
        /// <param name="durationMinutes">Thời lượng cuộc họp (phút)</param>
        /// <param name="attendeeEmail">Email người tham gia</param>
        /// <returns>Link Google Meet</returns>
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateMeeting(
            string title, 
            string description, 
            DateTime startTime, 
            int durationMinutes = 60, 
            string attendeeEmail = null)
        {
            try
            {
                var endTime = startTime.AddMinutes(durationMinutes);
                
                string[] attendees = null;
                if (!string.IsNullOrEmpty(attendeeEmail))
                {
                    attendees = new[] { attendeeEmail };
                }
                
                var meetingLink = await _googleMeetService.CreateMeetingAsync(
                    title,
                    description,
                    startTime,
                    endTime,
                    attendees);

                if (string.IsNullOrEmpty(meetingLink))
                {
                    return BadRequest(new ApiResponse
                    {
                        Code = 400,
                        Status = false,
                        Message = "Không thể tạo link Google Meet",
                        Data = null
                    });
                }

                return Ok(new ApiResponse
                {
                    Code = 200,
                    Status = true,
                    Message = "Đã tạo link Google Meet thành công",
                    Data = new { MeetingLink = meetingLink }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Code = 400,
                    Status = false,
                    Message = "Lỗi khi tạo Google Meet",
                    Data = ex.Message
                });
            }
        }
    }
} 