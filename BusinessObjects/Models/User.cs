using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class User
{
    public int Userid { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Avatar { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Customerform> Customerforms { get; set; } = new List<Customerform>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Lawyer> Lawyers { get; set; } = new List<Lawyer>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Usercredit> Usercredits { get; set; } = new List<Usercredit>();
}
