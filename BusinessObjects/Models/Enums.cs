namespace BusinessObjects.Models
{
    public enum UserRole
    {
        Customer,
        Lawyer,
        Admin
    }

    // Đã chuyển sang sử dụng kiểu string
    // public enum ServiceType
    // {
    //     Lawform,
    //     Appointment
    // }

    // Đã chuyển sang sử dụng kiểu string
    // public enum LawType
    // {
    //     RealEstateLaw,
    //     CriminalLaw,
    //     LaborLaw,
    //     EnterpriseLaw
    // }

    // Đã chuyển sang sử dụng kiểu int
    // public enum DurationType
    // {
    //     Minutes30 = 30,
    //     Minutes60 = 60,
    //     Minutes90 = 90,
    //     Minutes120 = 120
    // }

    public enum AppointmentStatus
    {
        Pending,
        Confirmed,
        Completed,
        Cancelled
    }

    public enum FormStatus
    {
        Draft,
        Submitted,
        Processing,
        Completed,
        Cancelled
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }
} 