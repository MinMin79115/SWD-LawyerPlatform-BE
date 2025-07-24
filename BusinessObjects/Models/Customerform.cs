using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Customerform
{
    public int Customerformid { get; set; }

    public int? Userid { get; set; }

    public int? Lawformid { get; set; }

    public string? Status { get; set; }

    public decimal Totalamount { get; set; }

    public string? Formdata { get; set; }

    public string? Linkform { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Lawform? Lawform { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual User? User { get; set; }
}
