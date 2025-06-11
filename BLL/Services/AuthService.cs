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

namespace Hospital_BE.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPatientRecordRepository _patientRecordRepository;
        private readonly IUserService _userService;
        private readonly JwtService _jwtService;
        private readonly ApplicationDbContext _context;

        public AuthService(
            IUserRepository userRepository,
            IPatientRecordRepository patientRecordRepository,
            IUserService userService,
            JwtService jwtService,
            ApplicationDbContext context)
        {
            _userRepository = userRepository;
            _patientRecordRepository = patientRecordRepository;
            _userService = userService;
            _jwtService = jwtService;
            _context = context;
        }

        public async Task<ServiceResult> PhoneExistsAsync(string phone)
        {
            bool isExists = await _userRepository.ExistsByPhoneAsync(phone);
            if(isExists) return ServiceResult.Error("Số điện thoại đã tồn tại trong hệ thống");
            return ServiceResult.Ok("");
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
                    Phone = model.Phone,
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
                    Phone = !string.IsNullOrEmpty(model.PhonePatient) ? model.PhonePatient : null,
                    Email = !string.IsNullOrEmpty(model.Email) ? model.Email : null,
                    HealthInsuranceNumber = !string.IsNullOrEmpty(model.HealthInsuranceNumber) ? model.HealthInsuranceNumber : null,
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
                    Phone = model.Phone
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
                // Tìm người dùng theo số điện thoại
                var user = await _userRepository.GetByPhoneAsync(model.Phone);
                
                if (user == null)
                {
                    return ServiceResult<LoginResponseDTO>.Error("Số điện thoại hoặc mật khẩu không đúng");
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
                    return ServiceResult<LoginResponseDTO>.Error("Số điện thoại hoặc mật khẩu không đúng");
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
    }
} 