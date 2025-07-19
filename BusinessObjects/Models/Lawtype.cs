using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Lawtype
{
    public int Lawtypeid { get; set; }
    
    public LawType LawType { get; set; }

    public virtual ICollection<Lawform> Lawforms { get; set; } = new List<Lawform>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
