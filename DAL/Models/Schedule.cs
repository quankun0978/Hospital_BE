using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_BE.DAL.Models
{
    /// <summary>
    /// Bảng lịch khám của bác sĩ
    /// </summary>
    [Table("Schedules")]
    public class Schedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Ngày khám của bác sĩ
        /// </summary>
        [Required]
        [Column("Date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Mã CodeKey thời gian khám (tham chiếu tới Allcodes.CodeKey với CodeType = TIME)
        /// </summary>
        [Required]
        [StringLength(50)]
        [Column("TimeType")]
        public string TimeType { get; set; }

        /// <summary>
        /// Mã Id của bác sĩ
        /// </summary>
        [Required]
        [Column("DoctorId")]
        public Guid DoctorId { get; set; }

        /// <summary>
        /// Trạng thái khả dụng của lịch khám (true: còn trống, false: đã được đặt)
        /// </summary>
        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        
        /// <summary>
        /// Thông tin bác sĩ
        /// </summary>
        [ForeignKey(nameof(DoctorId))]
        public virtual User Doctor { get; set; }

        /// <summary>
        /// Thông tin thời gian khám
        /// </summary>
        [ForeignKey(nameof(TimeType))]
        public virtual Allcode TimeTypeAllcode { get; set; }
    }
} 