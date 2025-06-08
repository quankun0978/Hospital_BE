using System;
using System.Collections.Generic;

namespace Hospital_BE.PL.DTOs
{
    /// <summary>
    /// DTO để hiển thị thông tin chi tiết của phòng khám/bệnh viện
    /// </summary>
    public class ClinicDetailDto
    {
        /// <summary>
        /// ID của phòng khám
        /// </summary>
        public Guid ClinicId { get; set; }

        /// <summary>
        /// Tên phòng khám/bệnh viện
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Đường dẫn thân thiện
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Hình ảnh đại diện
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Logo
        /// </summary>
        public string LogoImg { get; set; }

        /// <summary>
        /// Có phải là bệnh viện không
        /// </summary>
        public bool IsHospital { get; set; }

        /// <summary>
        /// Danh sách hình ảnh của phòng khám
        /// </summary>
        public List<ClinicImageDto> ClinicImages { get; set; }

        /// <summary>
        /// Danh sách bác sĩ làm việc tại phòng khám
        /// </summary>
        public List<ClinicDoctorDto> Doctors { get; set; }

        public List<ClinicSpecialtyDto> Specialties { get; set; }

    }

    /// <summary>
    /// DTO cho hình ảnh phòng khám
    /// </summary>
    public class ClinicImageDto
    {
        /// <summary>
        /// ID của hình ảnh
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// URL hình ảnh
        /// </summary>
        public string ImageFallbackUrl { get; set; }

        /// <summary>
        /// Có phải là hình nền không
        /// </summary>
        public bool IsBackground { get; set; }

        /// <summary>
        /// ID phòng khám
        /// </summary>
        public Guid ClinicId { get; set; }
    }

    /// <summary>
    /// DTO cho thông tin bác sĩ trong phòng khám
    /// </summary>
    public class ClinicDoctorDto
    {
        /// <summary>
        /// ID thông tin bác sĩ
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID bác sĩ
        /// </summary>
        public Guid DoctorId { get; set; }

        /// <summary>
        /// Tên bác sĩ
        /// </summary>
        public string DoctorName { get; set; }

        /// <summary>
        /// ID mức giá
        /// </summary>
        public string PriceId { get; set; }

        /// <summary>
        /// Tên mức giá
        /// </summary>
        public string PriceName { get; set; }

        /// <summary>
        /// ID vị trí
        /// </summary>
        public string PositionId { get; set; }

        /// <summary>
        /// Tên vị trí
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Đường dẫn
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Hình ảnh bác sĩ
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        public int? Count { get; set; }
    }

    /// <summary>
    /// DTO cho thông tin chuyên khoa trong phòng khám
    /// </summary>
    public class ClinicSpecialtyDto
    {
        /// <summary>
        /// ID chuyên khoa
        /// </summary>
        public Guid SpecialtyId { get; set; }

        /// <summary>
        /// Tên chuyên khoa
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Hình ảnh chuyên khoa
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Mô tả chuyên khoa
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Slug của chuyên khoa
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Số lượng bác sĩ có chuyên khoa này tại phòng khám
        /// </summary>
        public int DoctorCount { get; set; }

        /// <summary>
        /// Danh sách ID của các bác sĩ có chuyên khoa này
        /// </summary>
        public List<Guid> DoctorIds { get; set; }

        // Thêm các trường để tương thích với frontend
        public string Image => ImageUrl; // Alias cho ImageUrl
        public string Link => !string.IsNullOrEmpty(Slug) ? $"/chuyen-khoa/{Slug}" : "#"; // Tạo link từ slug
    }
} 