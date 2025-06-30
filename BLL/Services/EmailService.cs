using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Hospital_BE.BLL.Models;
using Hospital_BE.BLL.Interfaces;

namespace Hospital_BE.BLL.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IZipService _zipService;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        private readonly string _baseUrl;

        public EmailService(IConfiguration configuration, IZipService zipService)
        {
            _configuration = configuration;
            _zipService = zipService;
            _smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            _smtpUsername = _configuration["EmailSettings:SmtpUsername"] ?? "";
            _smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";
            _fromEmail = _configuration["EmailSettings:FromEmail"] ?? "";
            _fromName = _configuration["EmailSettings:FromName"] ?? "Hệ thống Bệnh viện";
            _baseUrl = _configuration["ApplicationSettings:BaseUrl"] ?? "https://localhost:7038";
        }

        public async Task<ServiceResult> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                using var client = new SmtpClient(_smtpHost, _smtpPort);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                return ServiceResult.Ok("Email đã được gửi thành công.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Error($"Lỗi khi gửi email: {ex.Message}");
            }
        }

        public async Task<ServiceResult> SendEmailWithAttachmentAsync(string toEmail, string subject, string body, Stream attachmentStream, string attachmentName, bool isHtml = true)
        {
            try
            {
                using var client = new SmtpClient(_smtpHost, _smtpPort);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                mailMessage.To.Add(toEmail);

                // Thêm attachment
                if (attachmentStream != null && attachmentStream.Length > 0)
                {
                    attachmentStream.Position = 0;
                    var attachment = new Attachment(attachmentStream, attachmentName, "application/zip");
                    mailMessage.Attachments.Add(attachment);
                }

                await client.SendMailAsync(mailMessage);
                return ServiceResult.Ok("Email với file đính kèm đã được gửi thành công.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Error($"Lỗi khi gửi email: {ex.Message}");
            }
        }

        public async Task<ServiceResult> SendAppointmentConfirmationAsync(
            string patientEmail, 
            string patientName, 
            string doctorName, 
            DateTime appointmentDate, 
            string timeSlot,
            string clinicName,
            Guid appointmentId,
            string reason = "")
        {
            try
            {
                var subject = "Xác nhận lịch khám - Hệ thống Bệnh viện";
                var body = GenerateAppointmentEmailTemplate(
                    patientName, 
                    doctorName, 
                    appointmentDate, 
                    timeSlot, 
                    clinicName,
                    appointmentId,
                    reason
                );

                return await SendEmailAsync(patientEmail, subject, body, true);
            }
            catch (Exception ex)
            {
                return ServiceResult.Error($"Lỗi khi gửi email xác nhận: {ex.Message}");
            }
        }

        private string GenerateAppointmentEmailTemplate(
            string patientName, 
            string doctorName, 
            DateTime appointmentDate, 
            string timeSlot, 
            string clinicName,
            Guid appointmentId,
            string reason)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #2563eb; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background: #f9f9f9; }}
        .info-table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
        .info-table th, .info-table td {{ padding: 12px; border: 1px solid #ddd; text-align: left; }}
        .info-table th {{ background: #f4f4f4; font-weight: bold; }}
        .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #666; }}
        .success {{ color: #28a745; font-weight: bold; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Xác nhận lịch khám</h1>
        </div>
        
        <div class='content'>
            <p>Xin chào <strong>{patientName}</strong>,</p>
            
            <p class='success'>✅ Lịch khám của bạn đã được đặt thành công!</p>
            
            <div style='background: #fff3cd; border: 1px solid #ffeaa7; border-radius: 8px; padding: 16px; margin: 20px 0;'>
                <p style='margin: 0; color: #856404; font-weight: bold;'>⚠️ Vui lòng xác nhận lịch khám</p>
                <p style='margin: 5px 0 0 0; color: #856404;'>Bạn cần xác nhận lịch khám để hoàn tất đặt lịch.</p>
            </div>
            
            <table class='info-table'>
                <tr>
                    <th>Bệnh nhân</th>
                    <td>{patientName}</td>
                </tr>
                <tr>
                    <th>Bác sĩ khám</th>
                    <td>{doctorName}</td>
                </tr>
                <tr>
                    <th>Ngày khám</th>
                    <td>{appointmentDate:dd/MM/yyyy}</td>
                </tr>
                <tr>
                    <th>Giờ khám</th>
                    <td>{timeSlot}</td>
                </tr>
                <tr>
                    <th>Địa điểm</th>
                    <td>{clinicName}</td>
                </tr>
                {(string.IsNullOrEmpty(reason) ? "" : $@"
                <tr>
                    <th>Lý do khám</th>
                    <td>{reason}</td>
                </tr>")}
            </table>
            
            <h3>Lưu ý quan trọng:</h3>
            <ul>
                <li>Vui lòng đến trước giờ hẹn 15-30 phút để làm thủ tục</li>
                <li>Mang theo CMND/CCCD và các giấy tờ y tế liên quan</li>
                <li>Nếu cần hủy hoặc thay đổi lịch hẹn, vui lòng liên hệ trước 24h</li>
                <li>Hotline hỗ trợ: 1900-2805</li>
            </ul>
            
            <div style='text-align: center; margin: 30px 0;'>
                <a href='http://localhost:5173/confirm-appointment?id={appointmentId}' 
                   style='background: #28a745; color: white; padding: 15px 30px; text-decoration: none; border-radius: 8px; font-weight: bold; display: inline-block;'>
                    XÁC NHẬN LỊCH KHÁM
                </a>
                <p style='margin-top: 10px; font-size: 12px; color: #666;'>
                    Hoặc copy link sau vào trình duyệt:<br/>
                    http://localhost:5173/confirm-appointment?id={appointmentId}
                </p>
            </div>
            
            <p>Cảm ơn bạn đã tin tưởng dịch vụ của chúng tôi!</p>
        </div>
        
        <div class='footer'>
            <p>© 2024 Hệ thống Bệnh viện. Mọi quyền được bảo lưu.</p>
            <p>Email này được gửi tự động, vui lòng không trả lời trực tiếp.</p>
        </div>
    </div>
</body>
</html>";
        }

        public async Task<ServiceResult> SendAppointmentCancellationAsync(
            string patientEmail,
            string patientName,
            string doctorName,
            DateTime appointmentDate,
            string timeSlot)
        {
            try
            {
                var subject = "Thông báo hủy lịch khám - Hệ thống Bệnh viện";
                var body = $@"
<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
    <h2 style='color: #dc3545;'>Thông báo hủy lịch khám</h2>
    <p>Xin chào <strong>{patientName}</strong>,</p>
    <p>Lịch khám của bạn đã được hủy với thông tin:</p>
    <ul>
        <li><strong>Bác sĩ:</strong> {doctorName}</li>
        <li><strong>Ngày:</strong> {appointmentDate:dd/MM/yyyy}</li>
        <li><strong>Giờ:</strong> {timeSlot}</li>
    </ul>
    <p>Nếu bạn cần đặt lịch khám mới, vui lòng truy cập website hoặc liên hệ hotline 1900-2805.</p>
    <p>Cảm ơn bạn!</p>
</div>";

                return await SendEmailAsync(patientEmail, subject, body, true);
            }
            catch (Exception ex)
            {
                return ServiceResult.Error($"Lỗi khi gửi email hủy lịch: {ex.Message}");
            }
        }

        public async Task<ServiceResult> SendMedicalResultsAsync(
            string patientEmail,
            string patientName,
            string doctorName,
            DateTime appointmentDate,
            string timeTypeText,
            string medicalNotes,
            List<string> medicalImages)
        {
            try
            {
                var subject = "Kết quả khám bệnh - Hệ thống Bệnh viện";
                var body = GenerateMedicalResultsEmailTemplate(
                    patientName,
                    doctorName,
                    appointmentDate,
                    timeTypeText,
                    medicalNotes,
                    medicalImages?.Count ?? 0
                );

                // Nếu có ảnh, tạo file zip và đính kèm
                if (medicalImages != null && medicalImages.Count > 0)
                {
                    try
                    {
                        var zipFileName = $"KetQuaKham_{patientName.Replace(" ", "_")}_{appointmentDate:yyyyMMdd}.zip";
                        
                        using var zipStream = await _zipService.CreateZipFromImageUrlsAsync(medicalImages, zipFileName);
                        
                        if (zipStream.Length > 0)
                        {
                            return await SendEmailWithAttachmentAsync(patientEmail, subject, body, zipStream, zipFileName, true);
                        }
                        else
                        {
                            // Nếu không tạo được zip, gửi email thường
                            return await SendEmailAsync(patientEmail, subject, body, true);
                        }
                    }
                    catch (Exception zipEx)
                    {
                        Console.WriteLine($"Lỗi khi tạo file zip: {zipEx.Message}");
                        // Fallback: gửi email thường nếu tạo zip thất bại
                        return await SendEmailAsync(patientEmail, subject, body, true);
                    }
                }
                else
                {
                    // Không có ảnh, gửi email thường
                    return await SendEmailAsync(patientEmail, subject, body, true);
                }
            }
            catch (Exception ex)
            {
                return ServiceResult.Error($"Lỗi khi gửi email kết quả khám: {ex.Message}");
            }
        }

        private string GenerateMedicalResultsEmailTemplate(
            string patientName,
            string doctorName,
            DateTime appointmentDate,
            string timeTypeText,
            string medicalNotes,
            int imageCount)
        {
            var attachmentSection = "";
            if (imageCount > 0)
            {
                attachmentSection = $@"
                <div style='background: #e3f2fd; border-left: 4px solid #2196f3; padding: 16px; margin: 20px 0; border-radius: 4px;'>
                    <h3 style='margin-top: 0; color: #1976d2; display: flex; align-items: center;'>
                        📎 File đính kèm - Kết quả khám bệnh
                    </h3>
                    <p style='margin: 8px 0; color: #424242;'>
                        ✅ Đã đính kèm file zip chứa <strong>{imageCount}</strong> ảnh kết quả khám/đơn thuốc
                    </p>
                    <p style='margin: 8px 0 0 0; font-size: 14px; color: #666;'>
                        💡 Vui lòng tải file đính kèm để xem chi tiết kết quả khám bệnh
                    </p>
                </div>";
            }

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #28a745; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background: #f9f9f9; }}
        .info-table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
        .info-table th, .info-table td {{ padding: 12px; border: 1px solid #ddd; text-align: left; }}
        .info-table th {{ background: #f4f4f4; font-weight: bold; }}
        .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #666; }}
        .medical-notes {{ background: #e7f3ff; border: 1px solid #b3d9ff; border-radius: 8px; padding: 16px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Kết quả khám bệnh</h1>
        </div>
        
        <div class='content'>
            <p>Xin chào <strong>{patientName}</strong>,</p>
            
            <p>Dưới đây là kết quả khám bệnh của bạn:</p>
            
            <table class='info-table'>
                <tr>
                    <th>Bệnh nhân</th>
                    <td>{patientName}</td>
                </tr>
                <tr>
                    <th>Bác sĩ khám</th>
                    <td>{doctorName}</td>
                </tr>
                <tr>
                    <th>Ngày khám</th>
                    <td>{appointmentDate:dd/MM/yyyy}</td>
                </tr>
                <tr>
                    <th>Giờ khám</th>
                    <td>{timeTypeText}</td>
                </tr>
            </table>
            
            <div class='medical-notes'>
                <h3 style='margin-top: 0; color: #0056b3;'>📋 Kết quả và chẩn đoán:</h3>
                <p style='white-space: pre-line;'>{medicalNotes}</p>
            </div>
            
            {attachmentSection}
            
            <h3>Lưu ý quan trọng:</h3>
            <ul>
                <li>Vui lòng tuân thủ đúng chỉ định của bác sĩ</li>
                <li>Bảo quản đơn thuốc và kết quả khám cẩn thận</li>
                <li>Nếu có triệu chứng bất thường, vui lòng liên hệ ngay với bác sĩ</li>
                <li>Hotline hỗ trợ 24/7: 1900-2805</li>
            </ul>
            
            <p>Cảm ơn bạn đã tin tưởng và sử dụng dịch vụ của chúng tôi!</p>
        </div>
        
        <div class='footer'>
            <p>© 2024 Hệ thống Bệnh viện. Mọi quyền được bảo lưu.</p>
            <p>Email này được gửi tự động, vui lòng không trả lời trực tiếp.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
} 