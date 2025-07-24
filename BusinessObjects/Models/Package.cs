using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Package
{
    public int Packageid { get; set; }

    public string Packagename { get; set; } = null!;

    public int? Bookingcount { get; set; }

    public int? Lawformcount { get; set; }

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Payment? Payment { get; set; }
}
