using System;
using System.Collections.Generic;

namespace Hospital_BE.PL.DTOs
{
    /// <summary>
    /// DTO để hiển thị thông tin chi tiết của bác sĩ
    /// </summary>
    public class DoctorDto
    {
        /// <summary>
        /// ID của bác sĩ
        /// </summary>
        public Guid DoctorId { get; set; }

        /// <summary>
        /// Tên bác sĩ
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Vai trò
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// Tên vai trò
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Danh sách thông tin chi tiết của bác sĩ
        /// </summary>
        public List<DoctorInfoDto> DoctorInfos { get; set; }

        /// <summary>
        /// Danh sách chuyên khoa của bác sĩ
        /// </summary>
        public List<DoctorSpecialtyDto> Specialties { get; set; }

        /// <summary>
        /// Danh sách phòng khám mà bác sĩ làm việc
        /// </summary>
        public List<DoctorClinicDto> Clinics { get; set; }
    }

    /// <summary>
    /// DTO để hiển thị thông tin chi tiết của bác sĩ
    /// </summary>
    public class DoctorInfoDto
    {
        /// <summary>
        /// ID của thông tin
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID của bác sĩ
        /// </summary>
        public Guid DoctorId { get; set; }

        /// <summary>
        /// ID của giá
        /// </summary>
        public string PriceId { get; set; }

        /// <summary>
        /// Tên mức giá
        /// </summary>
        public string PriceName { get; set; }

        /// <summary>
        /// ID của vị trí
        /// </summary>
        public string PositionId { get; set; }

        /// <summary>
        /// Tên vị trí
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// ID của phòng khám
        /// </summary>
        public Guid? ClinicId { get; set; }

        /// <summary>
        /// Tên phòng khám
        /// </summary>
        public string ClinicName { get; set; }

        /// <summary>
        /// Đường dẫn
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Đường dẫn hình ảnh
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        public int? Count { get; set; }
    }

    /// <summary>
    /// DTO cho chuyên khoa của bác sĩ
    /// </summary>
    public class DoctorSpecialtyDto
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
        /// Slug chuyên khoa
        /// </summary>
        public string Slug { get; set; }

        // Alias cho frontend
        public string Image => ImageUrl;
        public string Link => !string.IsNullOrEmpty(Slug) ? $"/chuyen-khoa/{Slug}" : "#";
    }

    /// <summary>
    /// DTO cho phòng khám mà bác sĩ làm việc
    /// </summary>
    public class DoctorClinicDto
    {
        /// <summary>
        /// ID phòng khám
        /// </summary>
        public Guid ClinicId { get; set; }

        /// <summary>
        /// Tên phòng khám
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Địa chỉ phòng khám
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Mô tả phòng khám
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Slug phòng khám
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Hình ảnh phòng khám
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Logo phòng khám
        /// </summary>
        public string LogoImg { get; set; }

        /// <summary>
        /// Có phải bệnh viện hay không
        /// </summary>
        public bool IsHospital { get; set; }
    }
} 