using System;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.BLL.Models;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs;
using Hospital_BE.DAL.Context;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Collections.Concurrent;

namespace Hospital_BE.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPatientRecordRepository _patientRecordRepository;
        private readonly IUserService _userService;
        private readonly JwtService _jwtService;
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        // In-memory storage cho email verification codes (trong production nên dùng Redis)
        private static readonly ConcurrentDictionary<string, EmailVerificationInfo> _emailVerificationStore = new();

        public AuthService(
            IUserRepository userRepository,
            IPatientRecordRepository patientRecordRepository,
            IUserService userService,
            JwtService jwtService,
            ApplicationDbContext context,
            EmailService emailService)
        {
            _userRepository = userRepository;
            _patientRecordRepository = patientRecordRepository;
            _userService = userService;
            _jwtService = jwtService;
            _context = context;
            _emailService = emailService;
        }

        public async Task<ServiceResult> EmailExistsAsync(string email)
        {
            bool isExists = await _userRepository.ExistsByEmailAsync(email);
            if(isExists) return ServiceResult.Error("Email đã tồn tại trong hệ thống");
            return ServiceResult.Ok("");
        }

        public async Task<ServiceResult> SendEmailVerificationAsync(SendEmailVerificationDTO model)
        {
            try
            {
                // Kiểm tra email đã tồn tại chưa
                bool emailExists = await _userRepository.ExistsByEmailAsync(model.Email);
                if (emailExists)
                {
                    return ServiceResult.Error("Email đã được sử dụng");
                }

                // Tạo mã xác thực 6 số
                var verificationCode = new Random().Next(100000, 999999).ToString();

                // Lưu mã xác thực vào memory với thời gian hết hạn 5 phút
                var verificationInfo = new EmailVerificationInfo
                {
                    Code = verificationCode,
                    ExpirationTime = DateTime.Now.AddMinutes(5)
                };

                _emailVerificationStore.AddOrUpdate(model.Email, verificationInfo, (key, oldValue) => verificationInfo);

                // Gửi email
                var emailSubject = "Mã xác thực đăng ký tài khoản";
                var emailBody = $"Mã xác thực của bạn là: <strong>{verificationCode}</strong><br/>Mã này sẽ hết hạn sau 5 phút.";
                
                await _emailService.SendEmailAsync(model.Email, emailSubject, emailBody);

                return ServiceResult.Ok("Mã xác thực đã được gửi đến email của bạn");
            }
            catch (Exception ex)
            {
                return ServiceResult.Error($"Lỗi khi gửi email xác thực: {ex.Message}");
            }
        }

        public async Task<ServiceResult> VerifyEmailAsync(VerifyEmailDTO model)
        {
            try
            {
                // Kiểm tra mã xác thực
                if (!_emailVerificationStore.TryGetValue(model.Email, out var verificationInfo))
                {
                    return ServiceResult.Error("Mã xác thực không tồn tại hoặc đã hết hạn");
                }

                if (DateTime.Now > verificationInfo.ExpirationTime)
                {
                    _emailVerificationStore.TryRemove(model.Email, out _);
                    return ServiceResult.Error("Mã xác thực đã hết hạn");
                }

                if (verificationInfo.Code != model.VerificationCode)
                {
                    return ServiceResult.Error("Mã xác thực không chính xác");
                }

                // Xác thực thành công, xóa mã khỏi memory
                _emailVerificationStore.TryRemove(model.Email, out _);

                return ServiceResult.Ok("Xác thực email thành công");
            }
            catch (Exception ex)
            {
                return ServiceResult.Error($"Lỗi khi xác thực email: {ex.Message}");
            }
        }

        public async Task<ServiceResult<RegisterResultDTO>> RegisterAsync(RegisterDTO model)
        {
            try
            {
                // Tạo User mới với password đã hash
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                var createUserDto = new CreateUserDTO
                {
                    Password = hashedPassword,
                    Name = model.Name,
                    Email = model.Email,
                    RoleId = model.RoleId
                };

                var userResult = await _userService.CreateUserAsync(createUserDto);

                if (!userResult.Success)
                {
                    return ServiceResult<RegisterResultDTO>.Error(userResult.Message);
                }

                // Tạo hồ sơ bệnh nhân liên kết với User vừa tạo
                var userId = Guid.Parse(userResult.Data);

                // Xử lý dữ liệu đầu vào đảm bảo không có giá trị null không hợp lệ
                var patientRecord = new PatientRecord
                {
                    UserId = userId,
                    FullName = !string.IsNullOrEmpty(model.FullName) ? model.FullName : model.Name,
                    DateOfBirth = model.DateOfBirth,
                    Gender = !string.IsNullOrEmpty(model.Gender) ? model.Gender : null,
                    Address = !string.IsNullOrEmpty(model.Address) ? model.Address : null,
                    Phone = !string.IsNullOrEmpty(model.Phone) ? model.Phone : null,
                    Email = !string.IsNullOrEmpty(model.Email) ? model.Email : null,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsActive = true
                };

                try
                {
                    await _patientRecordRepository.CreateAsync(patientRecord);
                }
                catch (Exception saveEx)
                {
                    Console.WriteLine($"Lỗi khi lưu PatientRecord: {saveEx.Message}");
                    if (saveEx.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {saveEx.InnerException.Message}");
                    }
                    throw;
                }

                // Trả về kết quả đăng ký thành công
                var result = new RegisterResultDTO
                {
                    UserId = userId.ToString(),
                    PatientId = patientRecord.PatientId.ToString(),
                    Username = model.Username,
                    Name = model.Name,
                    Email = model.Email
                };

                return ServiceResult<RegisterResultDTO>.Ok("Đăng ký tài khoản thành công", result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong quá trình đăng ký: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return ServiceResult<RegisterResultDTO>.Error($"Lỗi khi đăng ký tài khoản: {ex.Message}. InnerException: {ex.InnerException?.Message}");
            }
        }

        public async Task<ServiceResult<LoginResponseDTO>> LoginAsync(LoginDTO model)
        {
            try
            {
                // Tìm người dùng theo email
                var user = await _userRepository.GetByEmailAsync(model.Email);
                
                if (user == null)
                {
                    return ServiceResult<LoginResponseDTO>.Error("Email hoặc mật khẩu không đúng");
                }

                // Kiểm tra mật khẩu với BCrypt
                bool isPasswordValid = false;
                try
                {
                    // Thử verify với BCrypt trước
                    isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);
                }
                catch (Exception)
                {
                    // Nếu lỗi BCrypt, có thể password chưa được hash, thử so sánh trực tiếp
                    isPasswordValid = (model.Password == user.Password);
                    
                    // Nếu password đúng nhưng chưa hash, hash lại và cập nhật
                    if (isPasswordValid)
                    {
                        user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                        await _context.SaveChangesAsync();
                    }
                }

                if (!isPasswordValid)
                {
                    return ServiceResult<LoginResponseDTO>.Error("Email hoặc mật khẩu không đúng");
                }

                // Tạo JWT tokens
                var tokenResponse = _jwtService.GenerateTokens(user);

                // Xóa refresh token cũ của user này (nếu có)
                var existingToken = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.UserId == user.UserId);
                
                if (existingToken != null)
                {
                    _context.RefreshTokens.Remove(existingToken);
                }

                // Lưu refresh token mới vào database
                var refreshToken = new RefreshToken
                {
                    Token = tokenResponse.RefreshToken,
                    UserId = user.UserId,
                    ExpiryDate = tokenResponse.RefreshTokenExpiry,
                    CreatedAt = DateTime.UtcNow
                };

                _context.RefreshTokens.Add(refreshToken);
                await _context.SaveChangesAsync();

                return ServiceResult<LoginResponseDTO>.Ok("Đăng nhập thành công", tokenResponse);
            }
            catch (Exception ex)
            {
                return ServiceResult<LoginResponseDTO>.Error($"Đăng nhập thất bại: {ex.Message}");
            }
        }

        public async Task<ServiceResult<RefreshTokenResponseDTO>> RefreshTokenAsync(RefreshTokenRequestDTO model)
        {
            try
            {
                // Tìm refresh token trong database
                var refreshToken = await _context.RefreshTokens
                    .Include(rt => rt.User)
                    .FirstOrDefaultAsync(rt => rt.Token == model.RefreshToken);

                if (refreshToken == null)
                {
                    return ServiceResult<RefreshTokenResponseDTO>.Error("Refresh token không hợp lệ");
                }

                // Kiểm tra token đã hết hạn chưa
                if (refreshToken.ExpiryDate <= DateTime.UtcNow)
                {
                    _context.RefreshTokens.Remove(refreshToken);
                    await _context.SaveChangesAsync();
                    return ServiceResult<RefreshTokenResponseDTO>.Error("Refresh token đã hết hạn");
                }

                // Tạo tokens mới
                var newTokens = _jwtService.GenerateTokens(refreshToken.User);

                // Cập nhật refresh token với token mới
                refreshToken.Token = newTokens.RefreshToken;
                refreshToken.ExpiryDate = newTokens.RefreshTokenExpiry;
                refreshToken.CreatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var response = new RefreshTokenResponseDTO
                {
                    AccessToken = newTokens.AccessToken,
                    RefreshToken = newTokens.RefreshToken,
                    AccessTokenExpiry = newTokens.AccessTokenExpiry,
                    RefreshTokenExpiry = newTokens.RefreshTokenExpiry
                };

                return ServiceResult<RefreshTokenResponseDTO>.Ok("Làm mới token thành công", response);
            }
            catch (Exception ex)
            {
                return ServiceResult<RefreshTokenResponseDTO>.Error($"Lỗi khi làm mới token: {ex.Message}");
            }
        }

        public Task<ServiceResult> PhoneExistsAsync(string phone)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResult> SendForgotPasswordEmailAsync(ForgotPasswordDTO model)
        {
            try
            {
                // Kiểm tra email có tồn tại trong hệ thống không
                var user = await _userRepository.GetByEmailAsync(model.Email);
                if (user == null)
                {
                    return ServiceResult.Error("Email không tồn tại trong hệ thống");
                }

                // Tạo reset token (sử dụng JWT hoặc GUID)
                var resetToken = Guid.NewGuid().ToString();
                
                // Lưu token vào shared service với thời gian hết hạn 30 phút
                var expirationTime = DateTime.Now.AddMinutes(30);
                ResetPasswordTokenService.StoreToken(resetToken, model.Email, expirationTime);

                // Gửi email với link reset password
                var resetLink = $"http://localhost:5173/reset-password?token={resetToken}";
                var emailSubject = "Đặt lại mật khẩu - Hệ thống Bệnh viện";
                var emailBody = GenerateResetPasswordEmailTemplate(user.Name, resetLink);
                
                await _emailService.SendEmailAsync(model.Email, emailSubject, emailBody, true);

                return ServiceResult.Ok("Link đặt lại mật khẩu đã được gửi đến email của bạn");
            }
            catch (Exception ex)
            {
                return ServiceResult.Error($"Lỗi khi gửi email reset password: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> ValidateResetPasswordTokenAsync(string token)
        {
            try
            {
                var email = await ResetPasswordTokenService.ValidateAndGetEmailAsync(token);
                if (string.IsNullOrEmpty(email))
                {
                    return ServiceResult<string>.Error("Token không hợp lệ hoặc đã hết hạn");
                }

                return ServiceResult<string>.Ok("Token hợp lệ", email);
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Error($"Lỗi khi xác thực token: {ex.Message}");
            }
        }

        private string GenerateResetPasswordEmailTemplate(string userName, string resetLink)
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
        .button {{ background: #2563eb; color: white; padding: 15px 30px; text-decoration: none; border-radius: 8px; font-weight: bold; display: inline-block; }}
        .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Đặt lại mật khẩu</h1>
        </div>
        
        <div class='content'>
            <p>Xin chào <strong>{userName}</strong>,</p>
            
            <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.</p>
            
            <p>Vui lòng nhấn vào nút bên dưới để đặt lại mật khẩu:</p>
            
            <div style='text-align: center; margin: 30px 0;'>
                <a href='{resetLink}' class='button'>ĐẶT LẠI MẬT KHẨU</a>
            </div>
            
            <p>Hoặc copy link sau vào trình duyệt:</p>
            <p>{resetLink}</p>
            
            <p><strong>Lưu ý:</strong> Link này sẽ hết hạn sau 30 phút.</p>
            
            <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
        </div>
        
        <div class='footer'>
            <p>© 2025 Hệ thống Bệnh viện. Mọi quyền được bảo lưu.</p>
        </div>
    </div>
</body>
</html>";
        }

        // Helper class để lưu thông tin email verification
        private class EmailVerificationInfo
        {
            public string Code { get; set; }
            public DateTime ExpirationTime { get; set; }
        }
    }
} 