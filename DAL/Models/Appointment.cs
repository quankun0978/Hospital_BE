using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_BE.DAL.Models
{
    /// <summary>
    /// Bảng thông tin lịch khám bệnh
    /// </summary>
    [Table("Appointments")]
    public class Appointment
    {
        /// <summary>
        /// Khóa chính, mã Id của lịch khám
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AppointmentId { get; set; }

        /// <summary>
        /// Mã Id bệnh nhân
        /// </summary>
        [Required]
        public Guid PatientId { get; set; }

        /// <summary>
        /// Mã Id bác sĩ
        /// </summary>
        [Required]
        public Guid DoctorId { get; set; }

        /// <summary>
        /// Thời gian hẹn khám
        /// </summary>
        [Required]
        public DateTime AppointmentDate { get; set; }

        /// <summary>
        /// Khung giờ hẹn khám (ví dụ "08:00–08:30")
        /// </summary>
        [StringLength(50)]
        public string? TimeType { get; set; }

        /// <summary>
        /// Lý do khám bệnh của bệnh nhân
        /// </summary>
        [StringLength(500)]
        public string? Reason { get; set; }

        /// <summary>
        /// Trạng thái ('S1'=Lịch hẹn mới, 'S2'=Đã xác nhận, 'S3'=Đã khám xong, 'S4'=Đã hủy)
        /// </summary>
        [Required]
        [StringLength(2)]
        public string Status { get; set; } = "S1";

        /// <summary>
        /// Thời gian tạo lịch hẹn
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Thời gian cập nhật lần cuối
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Tham chiếu đến bảng PatientRecords
        /// </summary>
        [ForeignKey(nameof(PatientId))]
        public virtual PatientRecord Patient { get; set; }

        /// <summary>
        /// Tham chiếu đến bảng Users (Doctor)
        /// </summary>
        [ForeignKey(nameof(DoctorId))]
        public virtual User Doctor { get; set; }


    }
} 