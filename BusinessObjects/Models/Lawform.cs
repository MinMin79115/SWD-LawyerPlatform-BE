using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Lawform
{
    public int Lawformid { get; set; }
    
    public int Lawyerid { get; set; }
    
    public string? Formcontent { get; set; }
    
    public FormStatus Status { get; set; }
    
    public string? Comment { get; set; }
    
    public DateTime? Submitdate { get; set; }
    
    public DateTime? Createdat { get; set; }
    
    public DateTime? Updatedat { get; set; }
    
    public string? Formpath { get; set; }
    
    public string? Lawtypename { get; set; }
    
    public decimal? Price { get; set; }
    
    public string? Servicestypename { get; set; }
    
    public virtual Lawyer Lawyer { get; set; } = null!;
    
    public virtual Lawtype? Lawtype { get; set; }
    
    public virtual Servicestype? Servicestype { get; set; }
    
    public virtual ICollection<Customerform> Customerforms { get; set; } = new List<Customerform>();
}
