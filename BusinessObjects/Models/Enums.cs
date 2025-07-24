namespace BusinessObjects.Models
{
    public enum UserRole
    {
        Customer,
        Lawyer,
        Admin
    }

    public enum AppointmentStatus
    {
        Pending,
        Confirmed,
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