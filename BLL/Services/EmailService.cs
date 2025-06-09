using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Hospital_BE.BLL.Models;

namespace Hospital_BE.BLL.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            _smtpUsername = _configuration["EmailSettings:SmtpUsername"] ?? "";
            _smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";
            _fromEmail = _configuration["EmailSettings:FromEmail"] ?? "";
            _fromName = _configuration["EmailSettings:FromName"] ?? "Hệ thống Bệnh viện";
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

        public async Task<ServiceResult> SendAppointmentConfirmationAsync(
            string patientEmail, 
            string patientName, 
            string doctorName, 
            DateTime appointmentDate, 
            string timeSlot,
            string clinicName,
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
    }
} 