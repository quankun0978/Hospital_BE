using System;
using System.ComponentModel.DataAnnotations;

namespace Hospital_BE.PL.DTOs
{
    /// <summary>
    /// DTO đăng ký tài khoản (kết hợp User và PatientRecord)
    /// </summary>
    public class RegisterDTO
    {
        // Thông tin User
        [StringLength(256, ErrorMessage = "Tên đăng nhập tối đa 256 ký tự")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(256, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [StringLength(255, ErrorMessage = "Họ và tên tối đa 255 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [StringLength(15, ErrorMessage = "Số điện thoại tối đa 15 ký tự")]
        [RegularExpression(@"^(0|\+84)(\d{9})$", ErrorMessage = "Số điện thoại không đúng định dạng")]
        public string? Phone { get; set; }

        public string? PhonePatient { get; set; }


        [Required(ErrorMessage = "Mã vai trò là bắt buộc")]
        [StringLength(50, ErrorMessage = "Mã vai trò tối đa 50 ký tự")]
        public string RoleId { get; set; } = "R2"; // Mặc định là bệnh nhân

        // Thông tin PatientRecord (có thể bổ sung thêm)
        [StringLength(255, ErrorMessage = "Họ và tên đầy đủ tối đa 255 ký tự")]
        public string FullName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(1)]
        [RegularExpression(@"^[MF]$", ErrorMessage = "Giới tính phải là 'M' (Nam) hoặc 'F' (Nữ)")]
        public string Gender { get; set; }

        [StringLength(255, ErrorMessage = "Địa chỉ tối đa 255 ký tự")]
        public string Address { get; set; }

        [StringLength(255, ErrorMessage = "Email tối đa 255 ký tự")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [StringLength(20, ErrorMessage = "Số bảo hiểm y tế tối đa 20 ký tự")]
        public string HealthInsuranceNumber { get; set; }
    }

    /// <summary>
    /// Kết quả đăng ký tài khoản
    /// </summary>
    public class RegisterResultDTO
    {
        public string UserId { get; set; }
        public string PatientId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }

    /// <summary>
    /// DTO đăng nhập
    /// </summary>
    public class LoginDTO
    {
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [StringLength(15, ErrorMessage = "Số điện thoại tối đa 15 ký tự")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string Password { get; set; }
    }

    /// <summary>
    /// DTO đáp ứng sau khi đăng nhập thành công
    /// </summary>
    public class LoginResponseDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpiry { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
        public UserDTO User { get; set; }
    }

    /// <summary>
    /// DTO cho refresh token request
    /// </summary>
    public class RefreshTokenRequestDTO
    {
        [Required(ErrorMessage = "Refresh token là bắt buộc")]
        public string RefreshToken { get; set; }
    }

    /// <summary>
    /// DTO cho refresh token response
    /// </summary>
    public class RefreshTokenResponseDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpiry { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
    }
} 