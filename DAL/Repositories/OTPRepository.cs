using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Hospital_BE.DAL.Interfaces;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Microsoft.Extensions.Configuration;

namespace Hospital_BE.DAL.Repositories
{
    public class OTPRepository : IOTPRepository
    {
        private readonly IConfiguration _configuration;
        
        // Lưu trữ OTP trong bộ nhớ với ConcurrentDictionary để đảm bảo thread-safety
        private static readonly ConcurrentDictionary<string, OTPInfo> _otpStore = new ConcurrentDictionary<string, OTPInfo>();
        private static readonly ConcurrentDictionary<string, string> _tempPasswordStore = new ConcurrentDictionary<string, string>();

        // Thời gian hết hạn mặc định cho OTP (5 phút)
        private readonly TimeSpan _otpExpiration = TimeSpan.FromMinutes(5);

        public OTPRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            
            // Khởi tạo Twilio với thông tin xác thực
            string accountSid = _configuration["Twilio:AccountSid"];
            string authToken = _configuration["Twilio:AuthToken"];
            
            if (!string.IsNullOrEmpty(accountSid) && !string.IsNullOrEmpty(authToken))
            {
                TwilioClient.Init(accountSid, authToken);
            }
        }

        public async Task<bool> SaveOTPAsync(string phone, string otpCode)
        {
            var otpInfo = new OTPInfo
            {
                OTPCode = otpCode,
                ExpirationTime = DateTime.UtcNow.Add(_otpExpiration)
            };

            _otpStore[phone] = otpInfo;
            
            // Gửi OTP qua SMS sử dụng Twilio
            try
            {
                string twilioPhoneNumber = _configuration["Twilio:PhoneNumber"];
                string message = $"Mã OTP của bạn là: {otpCode}. Mã này sẽ hết hạn sau 5 phút.";

                // Định dạng số điện thoại cho Twilio (thêm mã quốc gia +84)
                string formattedPhone = phone;
                if (phone.StartsWith("0"))
                {
                    formattedPhone = "+84" + phone.Substring(1);
                }

                // Gửi tin nhắn SMS
                if (!string.IsNullOrEmpty(twilioPhoneNumber))
                {
                    var messageResource = await MessageResource.CreateAsync(
                        body: message,
                        from: new PhoneNumber(twilioPhoneNumber),
                        to: new PhoneNumber(formattedPhone)
                    );
                    
                    return messageResource.Status.ToString() != "failed";
                }
                
                return true; // Trả về true nếu không có cấu hình Twilio (môi trường dev)
            }
            catch (Exception)
            {
                // Log lỗi ở đây nếu cần
                return false;
            }
        }

        public Task<bool> VerifyOTPAsync(string phone, string otpCode)
        {
            if (_otpStore.TryGetValue(phone, out var otpInfo))
            {
                if (otpInfo.OTPCode == otpCode && otpInfo.ExpirationTime > DateTime.UtcNow)
                {
                    return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }

        public Task<bool> SavePasswordTempAsync(string phone, string password)
        {
            _tempPasswordStore[phone] = password;
            return Task.FromResult(true);
        }

        public Task<string> GetTempPasswordAsync(string phone)
        {
            _tempPasswordStore.TryGetValue(phone, out var password);
            return Task.FromResult(password ?? string.Empty);
        }

        public Task<bool> RemoveTempDataAsync(string phone)
        {
            _otpStore.TryRemove(phone, out _);
            _tempPasswordStore.TryRemove(phone, out _);
            return Task.FromResult(true);
        }

        private class OTPInfo
        {
            public string OTPCode { get; set; }
            public DateTime ExpirationTime { get; set; }
        }
    }
} 