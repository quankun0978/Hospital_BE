using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_BE.DAL.Models
{
    /// <summary>
    /// Bảng lưu nội dung Markdown của bác sĩ/phòng khám
    /// </summary>
    [Table("Markdowns")]
    public class Markdown
    {
        /// <summary>
        /// Khóa chính, định danh duy nhất của bản ghi Markdown
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Mã Id bác sĩ, khóa ngoại đến User.UserId (không bắt buộc)
        /// </summary>
        public Guid? DoctorId { get; set; }

        /// <summary>
        /// Mã Id phòng khám, khóa ngoại đến Clinic.ClinicId (không bắt buộc)
        /// </summary>
        public Guid? ClinicId { get; set; }

        /// <summary>
        /// Nội dung ở định dạng HTML
        /// </summary>
        [Required]
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string ContentHTML { get; set; } = string.Empty;

        /// <summary>
        /// Nội dung ở định dạng Markdown
        /// </summary>
        [Required]
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string ContentMarkdown { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả ngắn/excerpt của nội dung (nếu cần)
        /// </summary>
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string? Description { get; set; }

        /// <summary>
        /// Navigation: Tham chiếu đến User (bác sĩ)
        /// </summary>
        [ForeignKey(nameof(DoctorId))]
        public virtual User? Doctor { get; set; }

        /// <summary>
        /// Navigation: Tham chiếu đến Clinic
        /// </summary>
        [ForeignKey(nameof(ClinicId))]
        public virtual Clinic? Clinic { get; set; }
    }
}
