using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_BE.DAL.Models
{
    /// <summary>
    /// Bảng thông tin người dùng trong hệ thống
    /// </summary>
    [Table("Users")]
    public class User
    {
        /// <summary>
        /// Khóa chính, định danh duy nhất của người dùng
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserId { get; set; }

        /// <summary>
        /// Tên đăng nhập, phải duy nhất trong hệ thống
        /// </summary>
        [StringLength(256)]
        public string? Username { get; set; }

        /// <summary>
        /// Mật khẩu đã được mã hóa
        /// </summary>
        [StringLength(256)]
        public string Password { get; set; }

     

        /// <summary>
        /// Địa chỉ email, phải duy nhất trong hệ thống
        /// </summary>
        [StringLength(255)]
        public string? Email { get; set; }

        /// <summary>
        /// Tên người dùng
        /// </summary>
        [StringLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// Mã vai trò của người dùng, tham chiếu đến CodeKey trong bảng Allcodes (loại ROLE)
        /// </summary>
        [StringLength(50)]
        public string RoleId { get; set; }

        /// <summary>
        /// Thông tin chi tiết của người dùng nếu họ là bác sĩ
        /// </summary>
        public virtual ICollection<DoctorInfo> DoctorInfos { get; set; }

        /// <summary>
        /// Tham chiếu đến bảng Allcode, lấy theo RoleId = CodeKey
        /// </summary>
        [ForeignKey(nameof(RoleId))]
        public virtual Allcode Role { get; set; }
    }
} 