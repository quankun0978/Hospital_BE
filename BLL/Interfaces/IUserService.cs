using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital_BE.BLL.Models;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.BLL.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Kiểm tra số điện thoại đã tồn tại chưa
        /// </summary>
        /// <param name="phone">Số điện thoại cần kiểm tra</param>
        /// <returns>true nếu đã tồn tại, false nếu chưa tồn tại</returns>
        Task<bool> CheckPhoneExistsAsync(string phone);
        
        /// <summary>
        /// Lấy danh sách tất cả người dùng
        /// </summary>
        /// <returns>Danh sách người dùng</returns>
        Task<List<UserDTO>> GetAllUsersAsync();
        
        /// <summary>
        /// Lấy danh sách người dùng có phân trang
        /// </summary>
        /// <param name="parameters">Tham số truy vấn</param>
        /// <returns>Kết quả phân trang</returns>
        Task<PaginatedResult<User>> GetUsersAsync(QueryParameters parameters);
        
        /// <summary>
        /// Lấy thông tin chi tiết một người dùng theo ID
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <returns>Thông tin người dùng hoặc null nếu không tìm thấy</returns>
        Task<UserDTO> GetUserByIdAsync(string id);
        
        /// <summary>
        /// Tạo mới một người dùng
        /// </summary>
        /// <param name="model">Thông tin người dùng cần tạo</param>
        /// <returns>Kết quả tạo người dùng</returns>
        Task<ServiceResult<string>> CreateUserAsync(CreateUserDTO model);
        
        /// <summary>
        /// Cập nhật thông tin người dùng
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <param name="model">Thông tin cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        Task<ServiceResult> UpdateUserAsync(string id, UpdateUserDTO model);
        
        /// <summary>
        /// Xóa một người dùng
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <returns>Kết quả xóa</returns>
        Task<ServiceResult> DeleteUserAsync(string id);

        /// <summary>
        /// Lấy thông tin chi tiết người dùng bao gồm cả thông tin bác sĩ nếu có
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <returns>Thông tin chi tiết người dùng</returns>
        Task<UserDTO> GetUserDetailAsync(string id);
    }
} 