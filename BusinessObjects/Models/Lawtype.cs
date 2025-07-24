using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Lawtype
{
    public int Lawtypeid { get; set; }

    public string Lawtype1 { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Lawform> Lawforms { get; set; } = new List<Lawform>();
}
