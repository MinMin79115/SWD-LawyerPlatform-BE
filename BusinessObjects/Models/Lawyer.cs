using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Lawyer
{
    public int Lawyerid { get; set; }
    
    public int Userid { get; set; }
    
    public int? Experience { get; set; }
    
    public string? Description { get; set; }
    
    public string? Qualification { get; set; }
    
    public string[]? Specialties { get; set; }
    
    public decimal? Rating { get; set; }
    
    public decimal? Consultationfee { get; set; }
    
    public DateTime? Createdat { get; set; }
    
    public DateTime? Updatedat { get; set; }
    
    public virtual User User { get; set; } = null!;
    
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
