using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Service
{
    public int Serviceid { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public decimal Price { get; set; }
    
    public int? Durationid { get; set; }
    
    public int? Lawtypeid { get; set; }
    
    public int? Servicestypeid { get; set; }
    
    public DateTime? Createdat { get; set; }
    
    public DateTime? Updatedat { get; set; }
    
    public int? Packageid { get; set; }
    
    public virtual Duration? Duration { get; set; }
    
    public virtual Lawtype? Lawtype { get; set; }
    
    public virtual Package? Package { get; set; }
    
    public virtual Servicestype? ServicestypeNameNavigation { get; set; }
    
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
