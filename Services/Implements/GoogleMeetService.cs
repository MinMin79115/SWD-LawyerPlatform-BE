using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BusinessObjects.Common;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Options;
using Services.Interfaces;

namespace Services.Implements
{
    public class GoogleMeetService : IGoogleMeetService
    {
        private readonly GoogleCalendarSettings _settings;
        private readonly CalendarService _calendarService;

        public GoogleMeetService(IOptions<GoogleCalendarSettings> settings)
        {
            _settings = settings.Value;
            _calendarService = InitializeCalendarService();
        }

        public async Task<string> CreateMeetingAsync(string summary, string description, DateTime startTime, DateTime endTime, string[] attendeeEmails = null)
        {
            try
            {
                // Tạo sự kiện mới
                var newEvent = new Event
                {
                    Summary = summary,
                    Description = description,
                    Start = new EventDateTime
                    {
                        DateTime = startTime,
                        TimeZone = "Asia/Ho_Chi_Minh"
                    },
                    End = new EventDateTime
                    {
                        DateTime = endTime,
                        TimeZone = "Asia/Ho_Chi_Minh"
                    },
                    // Thêm Google Meet vào sự kiện
                    ConferenceData = new ConferenceData
                    {
                        CreateRequest = new CreateConferenceRequest
                        {
                            RequestId = Guid.NewGuid().ToString(),
                            ConferenceSolutionKey = new ConferenceSolutionKey
                            {
                                Type = "hangoutsMeet"
                            }
                        }
                    }
                };

                // Thêm người tham gia nếu có
                if (attendeeEmails != null && attendeeEmails.Length > 0)
                {
                    var attendees = new List<EventAttendee>();
                    foreach (var email in attendeeEmails)
                    {
                        attendees.Add(new EventAttendee { Email = email });
                    }
                    newEvent.Attendees = attendees;
                }

                // Tạo sự kiện trong Google Calendar
                var request = _calendarService.Events.Insert(newEvent, _settings.CalendarId);
                request.ConferenceDataVersion = 1;
                var createdEvent = await request.ExecuteAsync();

                // Trả về link Google Meet
                return createdEvent.HangoutLink;
            }
            catch (Exception ex)
            {
                // Log lỗi và trả về null
                Console.WriteLine($"Lỗi khi tạo Google Meet: {ex.Message}");
                return null;
            }
        }

        private CalendarService InitializeCalendarService()
        {
            try
            {
                // Sử dụng Service Account để xác thực
                GoogleCredential credential;
                using (var stream = new FileStream(_settings.ServiceAccountKeyPath, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream)
                        .CreateScoped(CalendarService.Scope.Calendar);
                }

                // Tạo service
                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = _settings.ApplicationName
                });

                return service;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi khởi tạo Calendar Service: {ex.Message}");
                throw;
            }
        }
    }
} 