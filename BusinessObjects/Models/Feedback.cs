using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Feedback
{
    public int Feedbackid { get; set; }

    public int? Userid { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual User? User { get; set; }
}
