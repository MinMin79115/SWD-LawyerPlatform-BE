using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class EmailMessage
{
    public int Id { get; set; }
    
    public string To { get; set; } = null!;
    
    public string Subject { get; set; } = null!;
    
    public string Content { get; set; } = null!;
    
    public string? AttachmentPath { get; set; }
    
    public bool IsSent { get; set; }
    
    public DateTime? DateSent { get; set; }
}
