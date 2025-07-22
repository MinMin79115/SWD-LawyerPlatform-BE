using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Lawtype
{
    public int LawTypeID { get; set; }
    
    public string LawType { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public DateTime? Createdat { get; set; }
    
    public DateTime? Updatedat { get; set; }
    
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    
    public virtual ICollection<Lawform> Lawforms { get; set; } = new List<Lawform>();
}
