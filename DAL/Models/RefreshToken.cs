using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_BE.DAL.Models
{
    /// <summary>
    /// Bảng lưu trữ refresh token
    /// </summary>
    [Table("RefreshTokens")]
    public class RefreshToken
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Token string
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Token { get; set; }

        /// <summary>
        /// Thời gian hết hạn
        /// </summary>
        [Required]
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// User ID sở hữu token
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Tham chiếu đến User
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
    }
} 