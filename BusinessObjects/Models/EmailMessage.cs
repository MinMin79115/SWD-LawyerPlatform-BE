using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class EmailMessage
{
    public string To { get; set; } = null!;
    
    public string Subject { get; set; } = null!;
    
    public string Body { get; set; } = null!;
    
    public bool IsHtml { get; set; } = true;
    
    public List<EmailAttachment>? Attachments { get; set; }
}

public class EmailAttachment
{
    public byte[] Content { get; set; } = null!;
    
    public string FileName { get; set; } = null!;
    
    public string ContentType { get; set; } = null!;
}
