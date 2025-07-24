using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Appointment
{
    public int Appointmentid { get; set; }

    public int? Userid { get; set; }

    public int? Lawtypeid { get; set; }

    public int? Lawyerid { get; set; }

    public TimeOnly Scheduletime { get; set; }

    public DateOnly Scheduledate { get; set; }

    public string? Status { get; set; }

    public string? Meetinglink { get; set; }

    public decimal Totalamount { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Lawtype? Lawtype { get; set; }

    public virtual Lawyer? Lawyer { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual User? User { get; set; }
}
