using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hospital_BE.PL.DTOs
{
    /// <summary>
    /// DTO cho tạo lịch hẹn mới
    /// </summary>
    public class CreateAppointmentDTO
    {
        [Required(ErrorMessage = "Mã bệnh nhân không được để trống")]
        public Guid PatientId { get; set; }

        [Required(ErrorMessage = "Mã bác sĩ không được để trống")]
        public Guid DoctorId { get; set; }

        [Required(ErrorMessage = "Ngày hẹn không được để trống")]
        public DateTime AppointmentDate { get; set; }

        [StringLength(50, ErrorMessage = "Khung giờ không được vượt quá 50 ký tự")]
        public string? TimeType { get; set; }

        [StringLength(500, ErrorMessage = "Lý do khám không được vượt quá 500 ký tự")]
        public string? Reason { get; set; }
    }

    /// <summary>
    /// DTO cho hoàn thành khám bệnh
    /// </summary>
    public class CompleteAppointmentDTO
    {
        [Required(ErrorMessage = "Mã lịch hẹn không được để trống")]
        public Guid AppointmentId { get; set; }

        [Required(ErrorMessage = "Ghi chú kết quả khám không được để trống")]
        [StringLength(2000, ErrorMessage = "Ghi chú không được vượt quá 2000 ký tự")]
        public string MedicalNotes { get; set; } = string.Empty;

        /// <summary>
        /// Danh sách URL ảnh kết quả khám, đơn thuốc
        /// </summary>
        public List<string> MedicalImages { get; set; } = new List<string>();
    }

    /// <summary>
    /// DTO cho thông tin chi tiết lịch hẹn
    /// </summary>
    public class AppointmentDetailsDTO
    {
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public PatientInfoDTO? Patient { get; set; }
        public Guid DoctorId { get; set; }
        public DoctorInfoDTO? Doctor { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? TimeType { get; set; }
        public string? TimeTypeText { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusText { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO cho thông tin bệnh nhân trong lịch hẹn
    /// </summary>
    public class PatientInfoDTO
    {
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? HealthInsuranceNumber { get; set; }
        public string? IdentityNumber { get; set; }
        public string? Ethnicity { get; set; }
        public string? Occupation { get; set; }
        public string? PatientCode { get; set; }
    }

    /// <summary>
    /// DTO cho thông tin bác sĩ trong lịch hẹn
    /// </summary>
    public class DoctorInfoDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? SpecialtyName { get; set; }
        public string? PositionName { get; set; }
    }

    /// <summary>
    /// DTO cho cập nhật trạng thái lịch hẹn
    /// </summary>
    public class UpdateAppointmentStatusDTO
    {
        [Required(ErrorMessage = "Trạng thái không được để trống")]
        [RegularExpression("^S[1-4]$", ErrorMessage = "Trạng thái chỉ có thể là S1 (Lịch hẹn mới), S2 (Đã xác nhận), S3 (Đã khám xong), hoặc S4 (Đã hủy)")]
        public string Status { get; set; } = string.Empty;
    }
} 