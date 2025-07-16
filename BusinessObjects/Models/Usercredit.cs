using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Usercredit
{
    public int Usercreditid { get; set; }

    public int? Userid { get; set; }

    public int? Bookingremaining { get; set; }

    public int? Lawformremaining { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual User? User { get; set; }
}
