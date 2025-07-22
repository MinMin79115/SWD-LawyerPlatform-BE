using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Customerform
{
    public int Customerformid { get; set; }
    
    public int Userid { get; set; }
    
    public string? Formcontent { get; set; }
    
    public FormStatus Status { get; set; }
    
    public string? Comment { get; set; }
    
    public DateTime? Submitdate { get; set; }
    
    public DateTime? Createdat { get; set; }
    
    public DateTime? Updatedat { get; set; }
    
    public object? Formdata { get; set; }
    
    public int? Lawformid { get; set; }
    
    public string? Linkform { get; set; }
    
    public decimal? Totalamount { get; set; }
    
    public virtual User User { get; set; } = null!;
    
    public virtual Lawform? Lawform { get; set; }
    
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
