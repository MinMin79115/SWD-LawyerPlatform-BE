using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Service
{
    public int Serviceid { get; set; }

    public int? Servicestypeid { get; set; }

    public int? Lawtypeid { get; set; }

    public int? Durationid { get; set; }

    public decimal Price { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Duration? Duration { get; set; }

    public virtual Lawtype? Lawtype { get; set; }

    public virtual Servicestype? Servicestype { get; set; }
}
