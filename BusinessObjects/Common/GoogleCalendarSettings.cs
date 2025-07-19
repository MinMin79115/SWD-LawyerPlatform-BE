namespace BusinessObjects.Common
{
    public class GoogleCalendarSettings
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string ServiceAccountKeyPath { get; set; } = string.Empty;
        public string CalendarId { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = "Lawyer Platform";
    }
} 