using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_BE.DAL.Models
{
    /// <summary>
    /// Bảng thông tin bệnh nhân
    /// </summary>
    [Table("PatientRecords")]
    public class PatientRecord
    {
        /// <summary>
        /// Khóa chính, định danh duy nhất của bệnh nhân
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PatientId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Id người dùng tạo hồ sơ
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Họ và tên đầy đủ của bệnh nhân
        /// </summary>
        [Required]
        [StringLength(255)]
        public string FullName { get; set; }

        /// <summary>
        /// Ngày sinh của bệnh nhân
        /// </summary>
        [Column(TypeName = "DATE")]
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Giới tính ('M' = Nam, 'F' = Nữ)
        /// </summary>
        [StringLength(1)]
        [Column(TypeName = "nchar(1)")]
        public string? Gender { get; set; }

        /// <summary>
        /// Địa chỉ liên hệ
        /// </summary>
        [StringLength(255)]
        public string? Address { get; set; }

        /// <summary>
        /// Số điện thoại liên lạc
        /// </summary>
        [StringLength(15)]
        public string? Phone { get; set; }

        /// <summary>
        /// Địa chỉ email
        /// </summary>
        [StringLength(255)]
        public string? Email { get; set; }

        /// <summary>
        /// Số bảo hiểm y tế (nếu có)
        /// </summary>
        [StringLength(20)]
        public string? HealthInsuranceNumber { get; set; }

        /// <summary>
        /// Số CMND/CCCD
        /// </summary>
        [StringLength(20)]
        public string? IdentityNumber { get; set; }

        /// <summary>
        /// Dân tộc
        /// </summary>
        [StringLength(50)]
        public string? Ethnicity { get; set; }

        /// <summary>
        /// Nghề nghiệp
        /// </summary>
        [StringLength(100)]
        public string? Occupation { get; set; }

        /// <summary>
        /// Mã bệnh nhân
        /// </summary>
        [StringLength(20)]
        public string? PatientCode { get; set; }

        /// <summary>
        /// Thời gian tạo bản ghi
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Thời gian cập nhật lần cuối
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Cờ trạng thái (0 = còn hiệu lực, 1 = đã xóa)
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Tham chiếu đến bảng User, lấy theo UserId
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
    }
} 