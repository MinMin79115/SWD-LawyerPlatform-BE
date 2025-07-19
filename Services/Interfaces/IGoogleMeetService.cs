using System;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IGoogleMeetService
    {
        /// <summary>
        /// Tạo một cuộc họp Google Meet mới và trả về link
        /// </summary>
        /// <param name="summary">Tiêu đề cuộc họp</param>
        /// <param name="description">Mô tả cuộc họp</param>
        /// <param name="startTime">Thời gian bắt đầu</param>
        /// <param name="endTime">Thời gian kết thúc</param>
        /// <param name="attendeeEmails">Danh sách email người tham gia (có thể null)</param>
        /// <returns>Link Google Meet</returns>
        Task<string> CreateMeetingAsync(string summary, string description, DateTime startTime, DateTime endTime, string[] attendeeEmails = null);
    }
} 