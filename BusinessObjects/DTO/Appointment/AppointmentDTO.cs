using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.DTO.Appointment
{
    public class TimeSlotDTO
    {
        public string Time { get; set; } = string.Empty;
        public bool Available { get; set; }
    }

    public class ConsultationTypeDTO
    {
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
    }

    public class DurationOptionDTO
    {
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Minutes { get; set; }
    }

    public class ConsultationMethodDTO
    {
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public decimal PriceAdjustment { get; set; }
    }

    public class PriceCalculationDTO
    {
        public string ConsultationType { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
    }

    public class PriceInfoDTO
    {
        public string Price { get; set; } = string.Empty;
    }

    public class AppointmentRequestDTO
    {
        [Required]
        public string ConsultationType { get; set; } = string.Empty;
        
        [Required]
        public string SelectedDate { get; set; } = string.Empty;
        
        [Required]
        public string SelectedTime { get; set; } = string.Empty;
        
        [Required]
        public int Duration { get; set; }
        
        [Required]
        public string Method { get; set; } = string.Empty;
        
        public string? SelectedLawyer { get; set; }
    }

    public class AppointmentResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string AppointmentId { get; set; } = string.Empty;
        public AppointmentDetailsDTO? AppointmentDetails { get; set; }
    }

    public class AppointmentDetailsDTO
    {
        public int AppointmentId { get; set; }
        public string ConsultationType { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string Method { get; set; } = string.Empty;
        public string LawyerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string TotalAmount { get; set; } = string.Empty;
    }
}
