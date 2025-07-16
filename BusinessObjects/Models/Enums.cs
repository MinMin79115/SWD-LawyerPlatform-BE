namespace BusinessObjects.Models
{
    // Đã được định nghĩa trong User.cs
    // public enum UserRole
    // {
    //     Customer,
    //     Lawyer,
    //     Admin
    // }

    public enum ServiceType
    {
        BookConsultant,
        LawForm
    }

    public enum LawType
    {
        RealEstateLaw,
        CriminalLaw,
        LaborLaw,
        EnterpriseLaw
    }

    public enum DurationType
    {
        Minutes30,
        Minutes60,
        Minutes90,
        Minutes120
    }

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