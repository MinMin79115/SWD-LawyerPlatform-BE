using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Appointment
{
    public int Appointmentid { get; set; }
    
    public int Userid { get; set; }
    
    public int? Serviceid { get; set; }
    
    public int? Lawyerid { get; set; }
    
    public TimeSpan Scheduletime { get; set; }
    
    public DateOnly Scheduledate { get; set; }
    
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    
    public string? Meetinglink { get; set; }
    
    public decimal Totalamount { get; set; }
    
    public DateTime? Createdat { get; set; }
    
    public DateTime? Updatedat { get; set; }
    
    public virtual Lawyer? Lawyer { get; set; }
    
    public virtual Service? Service { get; set; }
    
    public virtual User User { get; set; } = null!;
    
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
