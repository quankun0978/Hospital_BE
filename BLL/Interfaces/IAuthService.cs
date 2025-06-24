using System.Threading.Tasks;
using Hospital_BE.BLL.Models;
using Hospital_BE.PL.DTOs;

namespace Hospital_BE.BLL.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Đăng ký tài khoản mới kèm hồ sơ bệnh nhân
        /// </summary>
        /// <param name="model">Thông tin đăng ký</param>
        /// <returns>Kết quả đăng ký</returns>
        Task<ServiceResult<RegisterResultDTO>> RegisterAsync(RegisterDTO model);

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="model">Thông tin đăng nhập</param>
        /// <returns>Kết quả đăng nhập</returns>
        Task<ServiceResult<LoginResponseDTO>> LoginAsync(LoginDTO model);

        /// <summary>
        /// Gửi mã xác thực email
        /// </summary>
        /// <param name="model">Email cần gửi mã xác thực</param>
        /// <returns>Kết quả gửi mã</returns>
        Task<ServiceResult> SendEmailVerificationAsync(SendEmailVerificationDTO model);

        /// <summary>
        /// Xác thực mã email
        /// </summary>
        /// <param name="model">Email và mã xác thức</param>
        /// <returns>Kết quả xác thực</returns>
        Task<ServiceResult> VerifyEmailAsync(VerifyEmailDTO model);

        /// <summary>
        /// Kiểm tra email đã tồn tại chưa
        /// </summary>
        /// <param name="email">Email cần kiểm tra</param>
        /// <returns>true nếu đã tồn tại, false nếu chưa tồn tại</returns>
        Task<ServiceResult> EmailExistsAsync(string email);

        /// <summary>
        /// Kiểm tra số điện thoại đã tồn tại chưa
        /// </summary>
        /// <param name="phone">Số điện thoại cần kiểm tra</param>
        /// <returns>true nếu đã tồn tại, false nếu chưa tồn tại</returns>
        Task<ServiceResult> PhoneExistsAsync(string phone);

        /// <summary>
        /// Refresh access token
        /// </summary>
        /// <param name="model">Refresh token request</param>
        /// <returns>Tokens mới</returns>
        Task<ServiceResult<RefreshTokenResponseDTO>> RefreshTokenAsync(RefreshTokenRequestDTO model);

        /// <summary>
        /// Gửi email reset password
        /// </summary>
        /// <param name="model">Email cần reset password</param>
        /// <returns>Kết quả gửi email</returns>
        Task<ServiceResult> SendForgotPasswordEmailAsync(ForgotPasswordDTO model);

        /// <summary>
        /// Xác thực token reset password
        /// </summary>
        /// <param name="token">Token reset password</param>
        /// <returns>Email của user nếu token hợp lệ</returns>
        Task<ServiceResult<string>> ValidateResetPasswordTokenAsync(string token);
    }
} 