using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Refreshtoken
{
    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public string Jwtid { get; set; } = null!;

    public int? Userid { get; set; }

    public DateTime Addeddate { get; set; }

    public DateTime Expirydate { get; set; }

    public bool? Isused { get; set; }

    public bool? Isrevoked { get; set; }

    public virtual User? User { get; set; }
}
