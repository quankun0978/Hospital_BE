using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs.Common;
using Hospital_BE.PL.DTOs;

namespace Hospital_BE.BLL.Interfaces
{
    public interface IClinicService
    {
        /// <summary>
        /// Lấy danh sách phòng khám có phân trang
        /// </summary>
        /// <param name="parameters">Tham số truy vấn</param>
        /// <returns>Kết quả phân trang</returns>
        Task<PaginatedResult<Clinic>> GetClinicsAsync(QueryParameters parameters);
        
        /// <summary>
        /// Lấy thông tin chi tiết một phòng khám theo ID
        /// </summary>
        /// <param name="id">ID của phòng khám</param>
        /// <returns>Thông tin phòng khám hoặc null nếu không tìm thấy</returns>
        Task<ClinicDetailDto> GetClinicByIdAsync(Guid id);
        
        /// <summary>
        /// Lấy thông tin chi tiết một phòng khám theo slug
        /// </summary>
        /// <param name="slug">Slug của phòng khám</param>
        /// <returns>Thông tin phòng khám hoặc null nếu không tìm thấy</returns>
        Task<ClinicDetailDto> GetClinicBySlugAsync(string slug);
        
        /// <summary>
        /// Lấy thông tin cơ bản của phòng khám theo ID (entity)
        /// </summary>
        /// <param name="id">ID của phòng khám</param>
        /// <returns>Entity phòng khám hoặc null nếu không tìm thấy</returns>
        Task<Clinic> GetClinicEntityByIdAsync(Guid id);
        
        /// <summary>
        /// Tạo mới một phòng khám
        /// </summary>
        /// <param name="clinic">Thông tin phòng khám cần tạo</param>
        /// <returns>Phòng khám đã tạo</returns>
        Task<Clinic> CreateClinicAsync(Clinic clinic);
        
        /// <summary>
        /// Cập nhật thông tin phòng khám
        /// </summary>
        /// <param name="id">ID của phòng khám</param>
        /// <param name="clinic">Thông tin cập nhật</param>
        /// <returns>true nếu thành công, false nếu thất bại</returns>
        Task<bool> UpdateClinicAsync(Guid id, Clinic clinic);
        
        /// <summary>
        /// Xóa một phòng khám
        /// </summary>
        /// <param name="id">ID của phòng khám</param>
        /// <returns>true nếu thành công, false nếu thất bại</returns>
        Task<bool> DeleteClinicAsync(Guid id);

        /// <summary>
        /// Thêm ảnh vào phòng khám
        /// </summary>
        /// <param name="clinicId">ID của phòng khám</param>
        /// <param name="imageUrl">URL của ảnh</param>
        /// <param name="isBackground">Có phải ảnh nền không</param>
        /// <returns>ClinicImage đã tạo</returns>
        Task<ClinicImage> AddClinicImageAsync(Guid clinicId, string imageUrl, bool isBackground = false);
    }
} 