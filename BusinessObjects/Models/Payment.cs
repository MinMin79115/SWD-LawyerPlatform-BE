using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Payment
{
    public int Paymentid { get; set; }

    public int? Userid { get; set; }

    public int? Appointmentid { get; set; }

    public int? Customerformid { get; set; }

    public int? Packageid { get; set; }

    public decimal Amount { get; set; }

    public string? Transactionid { get; set; }
    
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public DateTime? Paymentdate { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Appointment? Appointment { get; set; }

    public virtual Customerform? Customerform { get; set; }

    public virtual Package? Package { get; set; }

    public virtual User? User { get; set; }
}
