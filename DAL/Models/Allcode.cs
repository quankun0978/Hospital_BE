using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_BE.DAL.Models
{
    /// <summary>
    /// Bảng dữ liệu tham chiếu cho các loại mã (CodeTypes) như: ROLE, POSITION, PRICE, STATUS, TIME...
    /// </summary>
    [Table("Allcodes")]
    public class Allcode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Mã khóa, là giá trị duy nhất trong hệ thống. Được sử dụng làm khóa thay thế để tham chiếu từ các bảng khác.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string CodeKey { get; set; }

        /// <summary>
        /// Loại mã, xác định nhóm của mã này (ví dụ: ROLE, POSITION, PRICE, STATUS, TIME...)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string CodeType { get; set; }

        /// <summary>
        /// Giá trị hiển thị bằng tiếng Anh
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ValueEn { get; set; }

        /// <summary>
        /// Giá trị hiển thị bằng tiếng Việt
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ValueVi { get; set; }

        /// <summary>
        /// Danh sách các User sử dụng Allcode này làm Role
        /// </summary>
        public virtual ICollection<User> UserRoles { get; set; }

        /// <summary>
        /// Danh sách các DoctorInfo liên kết với Allcode thông qua PriceId
        /// </summary>
        public virtual ICollection<DoctorInfo> DoctorPrices { get; set; }

        /// <summary>
        /// Danh sách các DoctorInfo liên kết với Allcode thông qua PositionId
        /// </summary>
        public virtual ICollection<DoctorInfo> DoctorPositions { get; set; }

        /// <summary>
        /// Danh sách các Schedule sử dụng Allcode này làm TimeType
        /// </summary>
        public virtual ICollection<Schedule> ScheduleTimeTypes { get; set; }
    }
} 