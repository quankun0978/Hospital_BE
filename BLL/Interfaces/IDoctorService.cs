using System;
using System.Threading.Tasks;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.BLL.Interfaces
{
    public interface IDoctorService
    {
        /// <summary>
        /// Lấy danh sách bác sĩ có phân trang
        /// </summary>
        /// <param name="parameters">Tham số truy vấn</param>
        /// <returns>Kết quả phân trang</returns>
        Task<PaginatedResult<DoctorDto>> GetDoctorsAsync(QueryParameters parameters);
        
        /// <summary>
        /// Lấy thông tin chi tiết một bác sĩ theo ID
        /// </summary>
        /// <param name="id">ID của bác sĩ</param>
        /// <returns>Thông tin bác sĩ hoặc null nếu không tìm thấy</returns>
        Task<DoctorDto> GetDoctorByIdAsync(Guid id);
        
        /// <summary>
        /// Lấy thông tin chi tiết một bác sĩ theo slug
        /// </summary>
        /// <param name="slug">Slug của bác sĩ</param>
        /// <returns>Thông tin bác sĩ hoặc null nếu không tìm thấy</returns>
        Task<DoctorDto> GetDoctorBySlugAsync(string slug);
        
        /// <summary>
        /// Tạo mới thông tin bác sĩ
        /// </summary>
        /// <param name="doctorInfo">Thông tin bác sĩ cần tạo</param>
        /// <returns>Thông tin bác sĩ đã tạo</returns>
        Task<DoctorInfo> CreateDoctorInfoAsync(DoctorInfo doctorInfo);
        
        /// <summary>
        /// Cập nhật thông tin bác sĩ
        /// </summary>
        /// <param name="id">ID của bác sĩ</param>
        /// <param name="doctorInfo">Thông tin cập nhật</param>
        /// <returns>true nếu thành công, false nếu thất bại</returns>
        Task<bool> UpdateDoctorInfoAsync(Guid id, DoctorInfo doctorInfo);
        
        /// <summary>
        /// Xóa thông tin bác sĩ
        /// </summary>
        /// <param name="id">ID của bác sĩ</param>
        /// <returns>true nếu thành công, false nếu thất bại</returns>
        Task<bool> DeleteDoctorInfoAsync(Guid id);
    }
} 