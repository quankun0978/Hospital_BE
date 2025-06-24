using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.BLL.Interfaces
{
    public interface IAllCodeService
    {
        /// <summary>
        /// Lấy danh sách mã theo loại với phân trang
        /// </summary>
        /// <param name="codeType">Loại mã</param>
        /// <param name="parameters">Tham số phân trang</param>
        /// <returns>Kết quả phân trang</returns>
        Task<PaginatedResult<Allcode>> GetAllCodesByTypeAsync(string codeType, PaginationParameters parameters);
        
        /// <summary>
        /// Lấy tất cả loại mã có trong hệ thống
        /// </summary>
        /// <returns>Danh sách các loại mã</returns>
        Task<List<string>> GetAllCodeTypesAsync();
        
        /// <summary>
        /// Lấy danh sách tất cả mã
        /// </summary>
        /// <param name="parameters">Tham số truy vấn</param>
        /// <returns>Kết quả phân trang</returns>
        Task<PaginatedResult<Allcode>> GetAllCodesAsync(QueryParameters parameters);
        
        /// <summary>
        /// Lấy mã theo ID
        /// </summary>
        /// <param name="id">ID của mã</param>
        /// <returns>Thông tin mã hoặc null nếu không tìm thấy</returns>
        Task<Allcode> GetAllCodeByIdAsync(int id);
        
        /// <summary>
        /// Tạo mới một mã
        /// </summary>
        /// <param name="allcode">Thông tin mã cần tạo</param>
        /// <returns>Mã đã tạo</returns>
        Task<Allcode> CreateAllcodeAsync(Allcode allcode);
        
        /// <summary>
        /// Cập nhật thông tin mã
        /// </summary>
        /// <param name="id">ID của mã</param>
        /// <param name="allcode">Thông tin cập nhật</param>
        /// <returns>true nếu thành công, false nếu thất bại</returns>
        Task<bool> UpdateAllcodeAsync(int id, Allcode allcode);
        
        /// <summary>
        /// Xóa một mã
        /// </summary>
        /// <param name="id">ID của mã</param>
        /// <returns>true nếu thành công, false nếu thất bại</returns>
        Task<bool> DeleteAllcodeAsync(int id);
        
        /// <summary>
        /// Lấy danh sách mã theo loại (không phân trang) - dùng cho dropdown
        /// </summary>
        /// <param name="codeType">Loại mã</param>
        /// <returns>Danh sách mã</returns>
        Task<List<Allcode>> GetAllCodeOptionsByTypeAsync(string codeType);
    }
} 