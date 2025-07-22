using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Servicestype
{
    public int ServiceTypeID { get; set; }
    
    public string ServicesType { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public DateTime? Createdat { get; set; }
    
    public DateTime? Updatedat { get; set; }
    
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
