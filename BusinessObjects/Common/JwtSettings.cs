using System;

namespace BusinessObjects.Common
{
    public class JwtSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int DurationInMinutes { get; set; }
        public int RefreshTokenDurationInDays { get; set; } = 7; // Mặc định 7 ngày
    }
} 