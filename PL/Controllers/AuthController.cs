using System;
using System.Linq;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.PL.Controllers.Base;
using Hospital_BE.PL.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_BE.PL.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// API đăng ký tài khoản, đồng thời tạo hồ sơ bệnh nhân
        /// </summary>
        /// <param name="model">Thông tin đăng ký tài khoản kèm hồ sơ bệnh nhân</param>
        /// <returns>Kết quả đăng ký</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest<object>(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }

            var result = await _authService.RegisterAsync(model);
            
            if (!result.Success)
            {
                return ApiBadRequest<object>(result.Message);
            }

            return ApiOk(result.Data , 
                "Đăng ký tài khoản thành công");
        }

        /// <summary>
        /// API đăng nhập
        /// </summary>
        /// <param name="model">Thông tin đăng nhập</param>
        /// <returns>Token và thông tin người dùng</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest<object>(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }

            var result = await _authService.LoginAsync(model);
            
            if (!result.Success)
            {
                return ApiBadRequest<object>(result.Message);
            }

            return ApiOk(result.Data, "Đăng nhập thành công");
        }

        /// <summary>
        /// API gửi mã xác thực email
        /// </summary>
        /// <param name="model">Email cần gửi mã xác thực</param>
        /// <returns>Kết quả gửi mã</returns>
        [HttpPost("send-email-verification")]
        public async Task<IActionResult> SendEmailVerification([FromBody] SendEmailVerificationDTO model)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest<object>(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }

            var result = await _authService.SendEmailVerificationAsync(model);
            
            if (!result.Success)
            {
                return ApiBadRequest<object>(result.Message);
            }

            return ApiOk(result.Data, "Mã xác thực đã được gửi đến email của bạn");
        }

        /// <summary>
        /// API xác thực mã email
        /// </summary>
        /// <param name="model">Email và mã xác thực</param>
        /// <returns>Kết quả xác thực</returns>
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDTO model)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest<object>(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }

            var result = await _authService.VerifyEmailAsync(model);
            
            if (!result.Success)
            {
                return ApiBadRequest<object>(result.Message);
            }

            return ApiOk(result.Data, "Xác thực email thành công");
        }

        /// <summary>
        /// API kiểm tra email đã tồn tại chưa
        /// </summary>
        /// <param name="model">Email cần kiểm tra</param>
        /// <returns>true nếu đã tồn tại, false nếu chưa tồn tại</returns>
        [HttpPost("check-email")]
        public async Task<IActionResult> CheckEmail([FromBody] EmailDTO model)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest<object>(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }

            var result = await _authService.EmailExistsAsync(model.Email);
            return Ok(result);
        }

        /// <summary>
        /// API kiểm tra số điện thoại đã tồn tại chưa
        /// </summary>
        /// <param name="model">Số điện thoại cần kiểm tra</param>
        /// <returns>true nếu đã tồn tại, false nếu chưa tồn tại</returns>
        [HttpPost("check-phone")]
        public async Task<IActionResult> CheckPhone([FromBody] PhoneDTO model)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest<object>(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }

            var result = await _authService.PhoneExistsAsync(model.Phone);
            return Ok(result);
        }

        /// <summary>
        /// API refresh token
        /// </summary>
        /// <param name="model">Refresh token request</param>
        /// <returns>Access token và refresh token mới</returns>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest<object>(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }

            var result = await _authService.RefreshTokenAsync(model);
            
            if (!result.Success)
            {
                return ApiBadRequest<object>(result.Message);
            }

            return ApiOk(result.Data, "Làm mới token thành công");
        }

        /// <summary>
        /// API gửi email reset password
        /// </summary>
        /// <param name="model">Email cần reset password</param>
        /// <returns>Kết quả gửi email</returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest<object>(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }

            var result = await _authService.SendForgotPasswordEmailAsync(model);
            
            if (!result.Success)
            {
                return ApiBadRequest<object>(result.Message);
            }

            return ApiOk(new { success = true }, result.Message);
        }

        /// <summary>
        /// API validate token reset password
        /// </summary>
        /// <param name="token">Token cần validate</param>
        /// <returns>Kết quả validate</returns>
        [HttpGet("validate-reset-token")]
        public async Task<IActionResult> ValidateResetToken([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return ApiBadRequest<object>("Token là bắt buộc");
            }

            var result = await _authService.ValidateResetPasswordTokenAsync(token);
            
            if (!result.Success)
            {
                return ApiBadRequest<object>(result.Message);
            }

            return ApiOk(new { email = result.Data }, "Token hợp lệ");
        }
    }
} 