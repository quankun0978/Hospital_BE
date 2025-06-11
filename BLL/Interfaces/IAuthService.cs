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
    }
} 