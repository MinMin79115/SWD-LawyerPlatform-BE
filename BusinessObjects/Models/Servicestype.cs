using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Servicestype
{
    public int Servicetypeid { get; set; }

    public virtual ICollection<Lawform> Lawforms { get; set; } = new List<Lawform>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
