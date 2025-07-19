using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Duration
{
    public int Durationid { get; set; }
    
    public DurationType Value { get; set; }

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
