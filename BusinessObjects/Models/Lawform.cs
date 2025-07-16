using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Lawform
{
    public int Lawformid { get; set; }

    public int? Servicestypeid { get; set; }

    public int? Lawtypeid { get; set; }

    public decimal Price { get; set; }

    public string Formpath { get; set; } = null!;

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual ICollection<Customerform> Customerforms { get; set; } = new List<Customerform>();

    public virtual Lawtype? Lawtype { get; set; }

    public virtual Servicestype? Servicestype { get; set; }
}
