using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO.Email
{
    public class AppointmentConfirmationEmailModel
    {
        public string CustomerName { get; set; } = string.Empty;
        public string ConsultationType { get; set; } = string.Empty;
        public string AppointmentDate { get; set; } = string.Empty;
        public string AppointmentTime { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string ConsultationMethod { get; set; } = string.Empty;
        public string LawyerName { get; set; } = string.Empty;
        public string TotalAmount { get; set; } = string.Empty;
        public bool IsOnlineConsultation { get; set; }
        public string MeetingLink { get; set; } = string.Empty;
    }
}
