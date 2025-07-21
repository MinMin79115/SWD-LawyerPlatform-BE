using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Package
{
    public int Packageid { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public decimal? Price { get; set; }
    
    public int? Numberofservices { get; set; }
    
    public int? Bookingcount { get; set; }
    
    public int? Lawformcount { get; set; }
    
    public string? Packagename { get; set; }
    
    public DateTime? Createdat { get; set; }
    
    public DateTime? Updatedat { get; set; }
    
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
