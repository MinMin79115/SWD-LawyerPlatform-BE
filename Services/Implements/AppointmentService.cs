using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObjects.Common;
using BusinessObjects.DTO.Appointment;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implements
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(IUnitOfWork unitOfWork, ILogger<AppointmentService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<TimeSlotDTO>> GetTimeSlotsAsync(DateOnly? date = null, int? lawyerId = null)
        {
            try
            {
                // If no date is provided, use today's date
                var targetDate = date ?? DateOnly.FromDateTime(DateTime.Today);
                
                // Get all possible time slots
                var timeSlots = new List<TimeSlotDTO>();
                for (int hour = 8; hour <= 18; hour++)
                {
                    // Add slots for every hour from 8 AM to 6 PM
                    var time = new TimeOnly(hour, 0, 0);
                    var isAvailable = await _unitOfWork.AppointmentRepository.IsTimeSlotAvailableAsync(time, targetDate, lawyerId);
                    
                    timeSlots.Add(new TimeSlotDTO
                    {
                        Time = $"{hour:D2}:00",
                        Available = isAvailable
                    });
                }
                
                return timeSlots;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting time slots");
                throw;
            }
        }

        public async Task<IEnumerable<ConsultationTypeDTO>> GetConsultationTypesAsync()
        {
            try
            {
                // Get all law types directly from the repository
                var lawTypes = await _unitOfWork.Repository<Lawtype>().GetAllAsync();
                
                // Convert to ConsultationTypeDTO
                var consultationTypes = lawTypes.Select(lt => new ConsultationTypeDTO
                {
                    Value = lt.Lawtypeid.ToString(), // Using ID as value
                    Label = lt.Lawtype1,             // Using name as label
                    Description = $"Tư vấn về {lt.Lawtype1}",
                    BasePrice = 500000               // Default price
                }).ToList();
                
                return consultationTypes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting consultation types");
                throw;
            }
        }

        public async Task<IEnumerable<DurationOptionDTO>> GetDurationOptionsAsync()
        {
            try
            {
                // Get all durations
                var durations = await _unitOfWork.Repository<Duration>().GetAllAsync();
                
                // Convert to DurationOptionDTO
                var durationOptions = durations.Select(d => new DurationOptionDTO
                {
                    Value = d.Value.ToString(),
                    Label = $"{d.Value} phút",
                    Description = $"Buổi tư vấn kéo dài {d.Value} phút",
                    Minutes = d.Value
                }).ToList();
                
                return durationOptions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting duration options");
                throw;
            }
        }

        public async Task<IEnumerable<ConsultationMethodDTO>> GetConsultationMethodsAsync()
        {
            try
            {
                // Define consultation methods (Online and In-person)
                var consultationMethods = new List<ConsultationMethodDTO>
                {
                    new ConsultationMethodDTO
                    {
                        Value = "online",
                        Label = "Tư vấn trực tuyến",
                        Description = "Qua video call, linh hoạt thời gian và địa điểm",
                        Icon = "video",
                        PriceAdjustment = 0
                    },
                    new ConsultationMethodDTO
                    {
                        Value = "inperson",
                        Label = "Tư vấn trực tiếp",
                        Description = "Gặp mặt tại văn phòng luật sư",
                        Icon = "building",
                        PriceAdjustment = 200000
                    }
                };
                
                return consultationMethods;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting consultation methods");
                throw;
            }
        }

        public async Task<PriceInfoDTO> CalculateConsultationPriceAsync(string consultationType, string duration, string method)
        {
            try
            {
                decimal basePrice = 500000; // Default base price
                
                // Get base price from law type if available
                var lawtype = await _unitOfWork.Repository<Lawtype>().GetFirstOrDefaultAsync(lt => lt.Lawtype1 == consultationType);
                
                // Apply duration multiplier
                if (int.TryParse(duration, out int durationValue))
                {
                    // Calculate price based on duration (e.g., 60 minutes = base price, 30 minutes = 0.75 * base price, 90 minutes = 1.5 * base price)
                    decimal durationMultiplier = durationValue switch
                    {
                        30 => 0.75m,
                        60 => 1.0m,
                        90 => 1.5m,
                        120 => 1.8m,
                        _ => 1.0m
                    };
                    
                    basePrice *= durationMultiplier;
                }
                
                // Apply method adjustment
                if (method == "inperson")
                {
                    basePrice += 200000; // Additional cost for in-person consultation
                }
                
                return new PriceInfoDTO
                {
                    Price = basePrice.ToString("N0") // Format with thousand separators
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating consultation price");
                throw;
            }
        }

        public async Task<AppointmentResponseDTO> SubmitAppointmentAsync(AppointmentRequestDTO request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                
                // Parse date and time
                if (!DateOnly.TryParse(request.SelectedDate, out DateOnly scheduleDate))
                {
                    throw new ArgumentException("Invalid date format");
                }

                string[] timeParts = request.SelectedTime.Split(':');
                if (timeParts.Length != 2 || !int.TryParse(timeParts[0], out int hour) || !int.TryParse(timeParts[1], out int minute))
                {
                    throw new ArgumentException("Invalid time format");
                }
                
                TimeOnly scheduleTime = new TimeOnly(hour, minute, 0);
                
                // Check if the time slot is available
                bool isAvailable = await _unitOfWork.AppointmentRepository.IsTimeSlotAvailableAsync(
                    scheduleTime, 
                    scheduleDate, 
                    !string.IsNullOrEmpty(request.SelectedLawyer) ? int.Parse(request.SelectedLawyer) : (int?)null
                );
                
                if (!isAvailable)
                {
                    return new AppointmentResponseDTO
                    {
                        Success = false,
                        Message = "Thời gian này đã được đặt. Vui lòng chọn thời gian khác."
                    };
                }
                
                // Calculate price
                var priceInfo = await CalculateConsultationPriceAsync(request.ConsultationType, request.Duration.ToString(), request.Method);
                decimal totalAmount = decimal.Parse(priceInfo.Price.Replace(",", ""));
                
                // Create new appointment
                var appointment = new Appointment
                {
                    Userid = request.UserId,
                    Lawyerid = !string.IsNullOrEmpty(request.SelectedLawyer) ? int.Parse(request.SelectedLawyer) : null,
                    Scheduledate = scheduleDate,
                    Scheduletime = scheduleTime,
                    // Status property is not available in Appointment model
                    // We need to store status in another way or add it to the model
                    Totalamount = totalAmount,
                    Createdat = DateTime.Now,
                    Updatedat = DateTime.Now
                };
                
                // Find lawtype ID first
                var lawtype = await _unitOfWork.Repository<Lawtype>().GetFirstOrDefaultAsync(
                    lt => lt.Lawtype1 == request.ConsultationType
                );
                
                if (lawtype == null)
                {
                    return null; // Handle error: law type not found
                }
                
                // Tìm Duration phù hợp dựa trên giá trị thời lượng
                var duration = await _unitOfWork.Repository<Duration>().GetFirstOrDefaultAsync(
                    d => d.Value == request.Duration
                );
                
                if (duration == null)
                {
                    // Nếu không tìm thấy Duration tương ứng, trả về lỗi
                    throw new Exception($"Không tìm thấy Duration với giá trị {request.Duration}");
                }
                
                // Find service using the IDs
                var service = await _unitOfWork.Repository<Service>().GetFirstOrDefaultAsync(
                    s => s.Lawtypeid == lawtype.Lawtypeid && 
                         s.Durationid == duration.Durationid
                );
                
                if (service == null)
                {
                    // Đã tìm Duration ở trên rồi, không cần tìm lại hay kiểm tra lại nữa
                    
                    // Create a new service entry if one doesn't exist
                    service = new Service
                    {
                        // Name and Description are not properties of Service model, we'll need to add them or use custom DTOs
                        Price = totalAmount,
                        Durationid = duration.Durationid,  // Sử dụng Durationid từ bản ghi Duration
                        Lawtypeid = lawtype.Lawtypeid,
                        Servicestypeid = 2, // Assuming 2 is the ID for Appointment service type
                        Createdat = DateTime.Now,
                        Updatedat = DateTime.Now
                    };
                    
                    await _unitOfWork.Repository<Service>().AddAsync(service);
                    await _unitOfWork.SaveChangesAsync();
                }
                
                appointment.Serviceid = service.Serviceid;
                
                // Add the appointment
                await _unitOfWork.Repository<Appointment>().AddAsync(appointment);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                
                // Get lawyer name if a lawyer was selected
                string lawyerName = null;
                if (appointment.Lawyerid.HasValue)
                {
                    var lawyer = await _unitOfWork.Repository<Lawyer>().GetByIdAsync(appointment.Lawyerid.Value);
                    if (lawyer != null)
                    {
                        var user = await _unitOfWork.Repository<User>().GetByIdAsync(lawyer.Userid);
                        lawyerName = user?.Name;
                    }
                }
                
                return new AppointmentResponseDTO
                {
                    Success = true,
                    Message = "Đặt lịch thành công! Chúng tôi sẽ liên hệ với bạn sớm nhất có thể.",
                    AppointmentId = appointment.Appointmentid.ToString(),
                    AppointmentDetails = new AppointmentDetailsDTO
                    {
                        AppointmentId = appointment.Appointmentid,
                        ConsultationType = request.ConsultationType,
                        Date = scheduleDate.ToString("dd/MM/yyyy"),
                        Time = request.SelectedTime,
                        Duration = request.Duration,
                        Method = request.Method == "online" ? "Trực tuyến" : "Trực tiếp",
                        LawyerName = lawyerName ?? "Chưa xác định",
                        Status = "Đang chờ xác nhận",
                        TotalAmount = totalAmount.ToString("N0") + " VNĐ"
                    }
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error submitting appointment");
                
                return new AppointmentResponseDTO
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi đặt lịch. Vui lòng thử lại sau."
                };
            }
        }

        public async Task<IEnumerable<AppointmentDetailsDTO>> GetUserAppointmentsAsync(int userId)
        {
            try
            {
                var appointments = await _unitOfWork.AppointmentRepository.GetAppointmentsByUserIdAsync(userId);
                
                // Helper method to get consultation type name
                string GetConsultationTypeFromService(Service service)
                {
                    if (service?.Lawtypeid == null) return "Unknown";
                    
                    var lawtype = _unitOfWork.Context.Lawtypes
                        .FirstOrDefault(l => l.Lawtypeid == service.Lawtypeid);
                        
                    return lawtype?.Lawtype1 ?? "Unknown";
                }
                
                return appointments.Select(a => new AppointmentDetailsDTO
                {
                    AppointmentId = a.Appointmentid,
                    ConsultationType = GetConsultationTypeFromService(a.Service),
                    Date = a.Scheduledate.ToString("dd/MM/yyyy"),
                    Time = a.Scheduletime.ToString(@"hh\:mm"),
                    Duration = a.Service?.Duration?.Value ?? 60,
                    Method = a.Meetinglink != null ? "Trực tuyến" : "Trực tiếp",
                    LawyerName = a.Lawyer?.User?.Name ?? "Chưa xác định",
                    Status = "Pending", // Status property missing in Appointment model
                    TotalAmount = a.Totalamount.ToString("N0") + " VNĐ"
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user appointments");
                throw;
            }
        }
    }
}
