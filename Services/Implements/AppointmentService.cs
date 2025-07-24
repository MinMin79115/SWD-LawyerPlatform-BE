using BusinessObjects.Common;
using BusinessObjects.DTO.Appointment;
using BusinessObjects.Models;
using Microsoft.Extensions.Options;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implements
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILawyerRepository _lawyerRepository;
        private readonly IEmailService _emailService;
        private readonly MeetingSettings _meetingSettings;

        public AppointmentService(
            IUnitOfWork unitOfWork,
            IAppointmentRepository appointmentRepository,
            IUserRepository userRepository,
            ILawyerRepository lawyerRepository,
            IEmailService emailService,
            IOptions<MeetingSettings> meetingSettings)
        {
            _unitOfWork = unitOfWork;
            _appointmentRepository = appointmentRepository;
            _userRepository = userRepository;
            _lawyerRepository = lawyerRepository;
            _emailService = emailService;
            _meetingSettings = meetingSettings.Value;
        }

        public async Task<ApiResponse> CreateAppointmentAsync(int userId, CreateAppointmentRequest request)
        {
            try
            {
                // Kiểm tra user tồn tại
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new ApiResponse
                    {
                        Code = 404,
                        Status = false,
                        Message = "Người dùng không tồn tại"
                    };
                }

                // Kiểm tra lawyer tồn tại
                var lawyer = await _lawyerRepository.GetByIdAsync(request.LawyerId);
                if (lawyer == null)
                {
                    return new ApiResponse
                    {
                        Code = 404,
                        Status = false,
                        Message = "Luật sư không tồn tại"
                    };
                }

                // Tạo appointment mới
                var appointment = new Appointment
                {
                    Userid = userId,
                    Lawtypeid = request.LawtypeId,
                    Lawyerid = request.LawyerId,
                    Scheduledate = request.ScheduleDate,
                    Scheduletime = request.ScheduleTime,
                    Status = "Pending",
                    Meetinglink = _meetingSettings.GoogleMeetUrl,
                    Totalamount = request.TotalAmount
                };

                var createdAppointment = await _appointmentRepository.CreateAppointmentAsync(appointment);
                await _unitOfWork.SaveChangesAsync();

                // Lấy thông tin chi tiết appointment vừa tạo
                var appointmentDetail = await _appointmentRepository.GetAppointmentByIdAsync(createdAppointment.Appointmentid);

                var response = MapToAppointmentResponse(appointmentDetail!);

                return new ApiResponse
                {
                    Code = 201,
                    Status = true,
                    Message = "Đặt lịch hẹn thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Code = 500,
                    Status = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse> GetAppointmentByIdAsync(int appointmentId)
        {
            try
            {
                var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
                if (appointment == null)
                {
                    return new ApiResponse
                    {
                        Code = 404,
                        Status = false,
                        Message = "Cuộc hẹn không tồn tại"
                    };
                }

                var response = MapToAppointmentResponse(appointment);

                return new ApiResponse
                {
                    Code = 200,
                    Status = true,
                    Message = "Lấy thông tin cuộc hẹn thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Code = 500,
                    Status = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse> GetAppointmentsByUserIdAsync(int userId)
        {
            try
            {
                var appointments = await _appointmentRepository.GetAppointmentsByUserIdAsync(userId);
                var response = appointments.Select(MapToAppointmentResponse).ToList();

                return new ApiResponse
                {
                    Code = 200,
                    Status = true,
                    Message = "Lấy danh sách cuộc hẹn thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Code = 500,
                    Status = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse> GetAppointmentsByLawyerIdAsync(int lawyerId)
        {
            try
            {
                var appointments = await _appointmentRepository.GetAppointmentsByLawyerIdAsync(lawyerId);
                var response = appointments.Select(MapToAppointmentResponse).ToList();

                return new ApiResponse
                {
                    Code = 200,
                    Status = true,
                    Message = "Lấy danh sách cuộc hẹn thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Code = 500,
                    Status = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse> GetAllAppointmentsAsync()
        {
            try
            {
                var appointments = await _appointmentRepository.GetAllAppointmentsAsync();
                var response = appointments.Select(MapToAppointmentResponse).ToList();

                return new ApiResponse
                {
                    Code = 200,
                    Status = true,
                    Message = "Lấy tất cả cuộc hẹn thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Code = 500,
                    Status = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse> UpdateAppointmentStatusAsync(int appointmentId, UpdateAppointmentStatusRequest request)
        {
            try
            {
                var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
                if (appointment == null)
                {
                    return new ApiResponse
                    {
                        Code = 404,
                        Status = false,
                        Message = "Cuộc hẹn không tồn tại"
                    };
                }

                appointment.Status = request.Status;
                await _appointmentRepository.UpdateAppointmentAsync(appointment);
                await _unitOfWork.SaveChangesAsync();

                var response = MapToAppointmentResponse(appointment);

                return new ApiResponse
                {
                    Code = 200,
                    Status = true,
                    Message = "Cập nhật trạng thái cuộc hẹn thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Code = 500,
                    Status = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse> GetLawyersAsync()
        {
            try
            {
                var lawyers = await _lawyerRepository.GetLawyersWithDetailsAsync();
                var response = lawyers.Select(l => new LawyerListResponse
                {
                    LawyerId = l.Lawyerid,
                    UserId = l.Userid ?? 0,
                    Name = l.User?.Name ?? "",
                    Email = l.User?.Email ?? "",
                    Experience = l.Experience,
                    Description = l.Description,
                    Qualification = l.Qualification,
                    Specialties = l.Specialties,
                    Rating = l.Rating,
                    ConsultationFee = l.Consultationfee,
                    Avatar = l.User?.Avatar
                }).ToList();

                return new ApiResponse
                {
                    Code = 200,
                    Status = true,
                    Message = "Lấy danh sách luật sư thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Code = 500,
                    Status = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse> GetLawtypesAsync()
        {
            try
            {
                var lawtypes = await _unitOfWork.LawtypeRepository.GetAllAsync();
                var response = lawtypes.Select(lt => new LawtypeListResponse
                {
                    LawtypeId = lt.Lawtypeid,
                    LawtypeName = lt.Lawtype1
                }).ToList();

                return new ApiResponse
                {
                    Code = 200,
                    Status = true,
                    Message = "Lấy danh sách loại pháp lý thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Code = 500,
                    Status = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse> ConfirmPaymentAsync(int appointmentId)
        {
            try
            {
                var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
                if (appointment == null)
                {
                    return new ApiResponse
                    {
                        Code = 404,
                        Status = false,
                        Message = "Cuộc hẹn không tồn tại"
                    };
                }

                // Cập nhật trạng thái thành Confirmed
                appointment.Status = "Confirmed";
                await _appointmentRepository.UpdateAppointmentAsync(appointment);
                await _unitOfWork.SaveChangesAsync();

                // Gửi email xác nhận
                await SendAppointmentConfirmationEmail(appointment);

                var response = MapToAppointmentResponse(appointment);

                return new ApiResponse
                {
                    Code = 200,
                    Status = true,
                    Message = "Xác nhận thanh toán và gửi email thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Code = 500,
                    Status = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }

        private async Task SendAppointmentConfirmationEmail(Appointment appointment)
        {
            try
            {
                var subject = "Xác nhận đặt lịch hẹn - Nền tảng Luật sư Basico";
                var body = $@"
                    <h2>Xác nhận đặt lịch hẹn thành công</h2>
                    <p>Kính chào {appointment.User?.Name},</p>
                    <p>Chúng tôi xác nhận rằng lịch hẹn của bạn đã được đặt thành công với các thông tin sau:</p>
                    <ul>
                        <li><strong>Mã cuộc hẹn:</strong> #{appointment.Appointmentid}</li>
                        <li><strong>Luật sư:</strong> {appointment.Lawyer?.User?.Name}</li>
                        <li><strong>Loại pháp lý:</strong> {appointment.Lawtype?.Lawtype1}</li>
                        <li><strong>Ngày hẹn:</strong> {appointment.Scheduledate:dd/MM/yyyy}</li>
                        <li><strong>Giờ hẹn:</strong> {appointment.Scheduletime:HH:mm}</li>
                        <li><strong>Số tiền:</strong> {appointment.Totalamount:N0} VNĐ</li>
                        <li><strong>Link meeting:</strong> <a href='{appointment.Meetinglink}'>Tham gia cuộc họp</a></li>
                    </ul>
                    <p>Vui lòng tham gia cuộc họp đúng giờ. Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>
                    <p>Trân trọng,<br>Đội ngũ Nền tảng Luật sư Basico</p>
                ";

                var emailMessage = new BusinessObjects.DTO.Email.EmailMessageDto
                {
                    To = appointment.User?.Email!,
                    Subject = subject,
                    Content = body
                };

                await _emailService.SendEmailAsync(emailMessage);
            }
            catch (Exception ex)
            {
                // Log error but don't throw - email failure shouldn't break the flow
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }

        private AppointmentResponse MapToAppointmentResponse(Appointment appointment)
        {
            return new AppointmentResponse
            {
                AppointmentId = appointment.Appointmentid,
                UserId = appointment.Userid ?? 0,
                UserName = appointment.User?.Name ?? "",
                UserEmail = appointment.User?.Email ?? "",
                LawtypeId = appointment.Lawtypeid ?? 0,
                LawtypeName = appointment.Lawtype?.Lawtype1 ?? "",
                LawyerId = appointment.Lawyerid ?? 0,
                LawyerName = appointment.Lawyer?.User?.Name ?? "",
                ScheduleDate = appointment.Scheduledate,
                ScheduleTime = appointment.Scheduletime,
                Status = appointment.Status ?? "",
                MeetingLink = appointment.Meetinglink,
                TotalAmount = appointment.Totalamount,
                CreatedAt = appointment.Createdat,
                UpdatedAt = appointment.Updatedat
            };
        }
    }
}
