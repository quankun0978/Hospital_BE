using System;
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
        /// Lấy danh sách tất cả người dùng
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return ApiOk(users, "Lấy danh sách người dùng thành công");
        }

        /// <summary>
        /// Lấy danh sách người dùng có phân trang
        /// </summary>
        /// <param name="parameters">Tham số phân trang</param>
        /// <returns>Danh sách người dùng đã phân trang</returns>
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] QueryParameters parameters)
        {
            var result = await _userService.GetUsersAsync(parameters);
            AddPaginationHeader(result);
            return ApiOk(result.Data, "Lấy danh sách người dùng thành công");
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

            return ApiOk(user, "Lấy chi tiết người dùng thành công");
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
    }
} 