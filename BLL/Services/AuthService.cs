using System;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.BLL.Models;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs;

namespace Hospital_BE.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPatientRecordRepository _patientRecordRepository;
        private readonly IUserService _userService;

        public AuthService(
            IUserRepository userRepository,
            IPatientRecordRepository patientRecordRepository,
            IUserService userService)
        {
            _userRepository = userRepository;
            _patientRecordRepository = patientRecordRepository;
            _userService = userService;
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
                // Tạo User mới
                var createUserDto = new CreateUserDTO
                {
                    Password = model.Password, // Trong thực tế, nên mã hóa mật khẩu ở đây
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

                // Kiểm tra mật khẩu (ở đây đang so sánh trực tiếp, thực tế nên mã hóa và so sánh)
                if (user.Password != model.Password)
                {
                    return ServiceResult<LoginResponseDTO>.Error("Số điện thoại hoặc mật khẩu không đúng");
                }

                // Lấy thông tin chi tiết của người dùng
                var userDto = await _userService.GetUserDetailAsync(user.UserId.ToString());

                // Tạo token (trong thực tế, nên sử dụng JWT hoặc phương pháp xác thực khác)
                var token = $"fake-jwt-token-{Guid.NewGuid()}"; // Đây chỉ là ví dụ, không nên sử dụng trong thực tế

                var response = new LoginResponseDTO
                {
                    Token = token,
                    User = userDto
                };

                return ServiceResult<LoginResponseDTO>.Ok("Đăng nhập thành công", response);
            }
            catch (Exception ex)
            {
                return ServiceResult<LoginResponseDTO>.Error($"Đăng nhập thất bại: {ex.Message}");
            }
        }
    }
} 