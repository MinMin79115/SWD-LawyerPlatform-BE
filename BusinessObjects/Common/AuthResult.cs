using System.Collections.Generic;

namespace BusinessObjects.Common
{
    public class AuthResult
    {
        public string Token { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
        public string RefreshToken { get; set; }
        
        public AuthResult()
        {
            Errors = new List<string>();
        }
    }
} 