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
    }
} 