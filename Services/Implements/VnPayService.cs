using BusinessObjects.Common;
using BusinessObjects.DTO.Payment;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Repositories.Interfaces;
using Services.Interfaces;
using System.Globalization;

namespace Services.Implements
{
    public class VnPayService : IVnPayService
    {
        private readonly VnPaySettings _vnPaySettings;
        private readonly IUnitOfWork _unitOfWork;

        public VnPayService(IOptions<VnPaySettings> vnPaySettings, IUnitOfWork unitOfWork)
        {
            _vnPaySettings = vnPaySettings.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse> CreatePaymentUrlAsync(CreateVnPayPaymentRequest request, HttpContext context)
        {
            try
            {
                var appointment = await _unitOfWork.AppointmentRepository.GetAppointmentByIdAsync(request.AppointmentId);
                if (appointment == null)
                {
                    return new ApiResponse
                    {
                        Code = 404,
                        Status = false,
                        Message = "Không tìm thấy lịch hẹn"
                    };
                }

                var existingPayment = await _unitOfWork.PaymentRepository.GetPaymentByAppointmentIdAsync(request.AppointmentId);
                if (existingPayment != null && existingPayment.Status == "Completed")
                {
                    return new ApiResponse
                    {
                        Code = 400,
                        Status = false,
                        Message = "Lịch hẹn đã được thanh toán"
                    };
                }

                Payment payment;
                
                if (existingPayment != null && existingPayment.Status == "Pending")
                {
                    payment = existingPayment;
                    payment.Amount = request.Amount;
                    payment.Updatedat = DateTime.Now;
                    
                    _unitOfWork.PaymentRepository.Update(payment);
                }
                else
                {
                    payment = new Payment
                    {
                        Userid = appointment.Userid,
                        Appointmentid = appointment.Appointmentid,
                        Amount = request.Amount,
                        Status = "Pending",
                        Createdat = DateTime.Now,
                        Updatedat = DateTime.Now
                    };

                    await _unitOfWork.PaymentRepository.AddAsync(payment);
                }
                
                await _unitOfWork.SaveChangesAsync();

                var paymentUrl = CreateVnPayUrl(payment, request, context);

                var response = new VnPayPaymentResponse
                {
                    PaymentUrl = paymentUrl,
                    PaymentId = payment.Paymentid.ToString(),
                    TransactionRef = payment.Paymentid.ToString()
                };

                return new ApiResponse
                {
                    Code = 200,
                    Status = true,
                    Message = "Tạo URL thanh toán thành công",
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

        public async Task<ApiResponse> ProcessReturnAsync(IQueryCollection vnpayData, string rawQueryString = null)
        {
            try
            {
                var vnpay = new VnPayLibrary();
                
                if (!string.IsNullOrEmpty(rawQueryString))
                {
                    var queryParams = rawQueryString.TrimStart('?').Split('&');
                    foreach (var param in queryParams)
                    {
                        var parts = param.Split('=', 2);
                        if (parts.Length == 2 && parts[0].StartsWith("vnp_"))
                        {
                            vnpay.AddResponseData(parts[0], parts[1]);
                        }
                    }
                }
                else
                {
                    foreach (var (key, value) in vnpayData)
                    {
                        if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                        {
                            vnpay.AddResponseData(key, value.ToString());
                        }
                    }
                }

                var vnpOrderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
                var vnpTransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                var vnpResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                var vnpTransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                var vnpSecureHash = vnpayData["vnp_SecureHash"].ToString();
                var vnpAmount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                var vnpBankCode = vnpay.GetResponseData("vnp_BankCode");
                var vnpPayDate = vnpay.GetResponseData("vnp_PayDate");

                bool checkSignature = vnpay.ValidateSignature(vnpSecureHash, _vnPaySettings.HashSecret);

                var response = new VnPayReturnResponse
                {
                    OrderId = vnpOrderId.ToString(),
                    TransactionId = vnpTransactionId.ToString(),
                    VnPayResponseCode = vnpResponseCode,
                    VnPayTransactionStatus = vnpTransactionStatus,
                    Amount = vnpAmount,
                    PaymentMethod = vnpBankCode
                };

                if (checkSignature)
                {
                    var payment = await _unitOfWork.PaymentRepository.GetPaymentByAppointmentIdAsync((int)vnpOrderId);
                    if (payment != null)
                    {
                        if (vnpResponseCode == "00" && vnpTransactionStatus == "00")
                        {
                            payment.Status = "Completed";
                            payment.Updatedat = DateTime.Now;
                            
                            response.Success = true;
                            response.Message = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                        }
                        else
                        {
                            payment.Status = "Failed";
                            payment.Updatedat = DateTime.Now;
                            
                            response.Success = false;
                            response.Message = $"Có lỗi xảy ra trong quá trình xử lý. Mã lỗi: {vnpResponseCode}";
                        }

                        _unitOfWork.PaymentRepository.Update(payment);
                        await _unitOfWork.SaveChangesAsync();
                    }

                    if (!string.IsNullOrEmpty(vnpPayDate))
                    {
                        response.PaymentDate = VnPayLibrary.ParseVnPayDate(vnpPayDate);
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "Chữ ký không hợp lệ";
                }

                return new ApiResponse
                {
                    Code = 200,
                    Status = true,
                    Message = "Xử lý kết quả thanh toán thành công",
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

        public async Task<string> ProcessIpnAsync(IQueryCollection vnpayData)
        {
            try
            {
                var vnpay = new VnPayLibrary();
                foreach (var (key, value) in vnpayData)
                {
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(key, value.ToString());
                    }
                }

                var vnpOrderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
                var vnpAmount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                var vnpTransactionId = vnpay.GetResponseData("vnp_TransactionNo");
                var vnpResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                var vnpTransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                var vnpSecureHash = vnpayData["vnp_SecureHash"].ToString();

                bool checkSignature = vnpay.ValidateSignature(vnpSecureHash, _vnPaySettings.HashSecret);

                if (checkSignature)
                {
                    // Lấy payment từ database
                    var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(vnpOrderId);
                    
                    if (payment != null)
                    {
                        if (payment.Amount == vnpAmount)
                        {
                            if (payment.Status == "Pending")
                            {
                                if (vnpResponseCode == "00" && vnpTransactionStatus == "00")
                                {
                                    payment.Status = "Completed";
                                    payment.Transactionid = vnpTransactionId;
                                    payment.Paymentdate = DateTime.Now;
                                    payment.Updatedat = DateTime.Now;

                                    if (payment.Appointment != null)
                                    {
                                        payment.Appointment.Status = "Confirmed";
                                        payment.Appointment.Updatedat = DateTime.Now;
                                    }

                                    await _unitOfWork.SaveChangesAsync();
                                    return "{\"RspCode\":\"00\",\"Message\":\"Confirm Success\"}";
                                }
                                else
                                {
                                    payment.Status = "Failed";
                                    payment.Transactionid = vnpTransactionId;
                                    payment.Updatedat = DateTime.Now;

                                    await _unitOfWork.SaveChangesAsync();
                                    return "{\"RspCode\":\"00\",\"Message\":\"Confirm Success\"}";
                                }
                            }
                            else
                            {
                                return "{\"RspCode\":\"02\",\"Message\":\"Order already confirmed\"}";
                            }
                        }
                        else
                        {
                            return "{\"RspCode\":\"04\",\"Message\":\"Invalid amount\"}";
                        }
                    }
                    else
                    {
                        return "{\"RspCode\":\"01\",\"Message\":\"Order not found\"}";
                    }
                }
                else
                {
                    return "{\"RspCode\":\"97\",\"Message\":\"Invalid signature\"}";
                }
            }
            catch (Exception)
            {
                return "{\"RspCode\":\"99\",\"Message\":\"Input data required\"}";
            }
        }

        public bool ValidateSignature(IQueryCollection vnpayData, string secretKey)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in vnpayData)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnpSecureHash = vnpayData["vnp_SecureHash"].ToString();
            return vnpay.ValidateSignature(vnpSecureHash, secretKey);
        }

        private string CreateVnPayUrl(Payment payment, CreateVnPayPaymentRequest request, HttpContext context)
        {
            var vnpay = new VnPayLibrary();
            var createDate = DateTime.Now;

            vnpay.AddRequestData("vnp_Version", _vnPaySettings.Version);
            vnpay.AddRequestData("vnp_Command", _vnPaySettings.Command);
            vnpay.AddRequestData("vnp_TmnCode", _vnPaySettings.TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((long)(request.Amount * 100)).ToString());
            vnpay.AddRequestData("vnp_CreateDate", createDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _vnPaySettings.CurrCode);
            vnpay.AddRequestData("vnp_IpAddr", VnPayLibrary.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _vnPaySettings.Locale);
            vnpay.AddRequestData("vnp_OrderInfo", $"Payment for appointment {payment.Appointmentid}");
            vnpay.AddRequestData("vnp_OrderType", _vnPaySettings.OrderType);
            vnpay.AddRequestData("vnp_ReturnUrl", _vnPaySettings.ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", payment.Paymentid.ToString());
            
            // Add ExpireDate (bắt buộc)
            vnpay.AddRequestData("vnp_ExpireDate", createDate.AddMinutes(_vnPaySettings.TimeoutMinutes).ToString("yyyyMMddHHmmss"));
            
            // Add BankCode nếu có
            if (!string.IsNullOrEmpty(request.BankCode))
            {
                vnpay.AddRequestData("vnp_BankCode", request.BankCode);
            }

            return vnpay.CreateRequestUrl(_vnPaySettings.BaseUrl, _vnPaySettings.HashSecret);
        }
    }
}
