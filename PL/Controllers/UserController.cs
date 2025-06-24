using System;
using System.Linq;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.PL.Controllers.Base;
using Hospital_BE.PL.DTOs;
using Hospital_BE.PL.DTOs.Common;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_BE.PL.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Lấy danh sách tất cả người dùng - tránh circular reference
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            
            // Tạo DTO để tránh circular reference
            var userDtos = users.Select(u => new UserResponseDto
            {
                UserId = Guid.Parse(u.UserId),
                Username = u.Username,
                Email = u.Email,
                Name = u.Name,
                RoleId = u.RoleId,
                RoleName = u.RoleName ?? "Không xác định"
            }).ToList();

            return ApiOk(userDtos, "Lấy danh sách người dùng thành công");
        }

        /// <summary>
        /// Lấy danh sách người dùng có phân trang - tránh circular reference
        /// </summary>
        /// <param name="parameters">Tham số phân trang</param>
        /// <returns>Danh sách người dùng đã phân trang</returns>
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] QueryParameters parameters)
        {
            var result = await _userService.GetUsersAsync(parameters);
            
            // Convert to DTO to avoid circular reference
            var userDtos = result.Data.Select(u => new UserResponseDto
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                Name = u.Name,
                RoleId = u.RoleId,
                RoleName = u.Role?.ValueVi ?? "Không xác định"
            }).ToList();

            // Create new paginated result with DTOs
            var dtoResult = new PaginatedResult<UserResponseDto>(
                userDtos,
                result.TotalCount,
                result.CurrentPage,
                result.PageSize
            );
            
            AddPaginationHeader(result);
            return ApiOk(dtoResult, "Lấy danh sách người dùng thành công");
        }

        /// <summary>
        /// Lấy thông tin chi tiết người dùng theo ID
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <returns>Thông tin chi tiết người dùng</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _userService.GetUserDetailAsync(id.ToString());

            if (user == null)
                return ApiNotFound<object>("Không tìm thấy người dùng");

            // Convert to DTO to avoid circular reference
            var userDto = new UserResponseDto
            {
                UserId = Guid.Parse(user.UserId),
                Username = user.Username,
                Email = user.Email,
                Name = user.Name,
                RoleId = user.RoleId,
                RoleName = user.RoleName ?? "Không xác định"
            };

            return ApiOk(userDto, "Lấy chi tiết người dùng thành công");
        }

        /// <summary>
        /// Tạo người dùng mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO model)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest<object>(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }

            var result = await _userService.CreateUserAsync(model);
            if (!result.Success)
            {
                return ApiBadRequest<object>(result.Message);
            }

            return ApiCreated(new { userId = result.Data }, nameof(GetUser), new { id = result.Data }, "Tạo người dùng thành công");
        }

        /// <summary>
        /// Cập nhật thông tin người dùng
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDTO model)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest<object>(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }

            var result = await _userService.UpdateUserAsync(id.ToString(), model);
            if (!result.Success)
            {
                if (result.Message.Contains("không tìm thấy"))
                {
                    return ApiNotFound<object>(result.Message);
                }
                return ApiBadRequest<object>(result.Message);
            }

            return ApiOk(new { success = true }, "Cập nhật người dùng thành công");
        }

        /// <summary>
        /// Xóa người dùng
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id.ToString());
            if (!result.Success)
            {
                if (result.Message.Contains("không tìm thấy"))
                {
                    return ApiNotFound<object>(result.Message);
                }
                return ApiBadRequest<object>(result.Message);
            }

            return ApiNoContent();
        }

        /// <summary>
        /// API reset password
        /// </summary>
        /// <param name="model">Thông tin reset password</param>
        /// <returns>Kết quả reset</returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest<object>(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }

            var result = await _userService.ResetPasswordAsync(model);
            if (!result.Success)
            {
                return ApiBadRequest<object>(result.Message);
            }

            return ApiOk(new { success = true }, result.Message);
        }

        /// <summary>
        /// API đổi mật khẩu
        /// </summary>
        /// <param name="id">ID người dùng</param>
        /// <param name="model">Thông tin đổi password</param>
        /// <returns>Kết quả đổi password</returns>
        [HttpPost("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest<object>(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }

            var result = await _userService.ChangePasswordAsync(id.ToString(), model);
            if (!result.Success)
            {
                return ApiBadRequest<object>(result.Message);
            }

            return ApiOk(new { success = true }, result.Message);
        }
    }

    /// <summary>
    /// DTO để tránh circular reference khi trả về thông tin User
    /// </summary>
    public class UserResponseDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }
} 