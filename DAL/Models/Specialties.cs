using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_BE.DAL.Models
{
    /// <summary>
    /// Bảng thông tin về chuyên khoa
    /// </summary>
    [Table("Specialties")]
    public class Specialty
    {
        /// <summary>
        /// Khóa chính của chuyên khoa, là định danh duy nhất
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SpecialtyId { get; set; }

        /// <summary>
        /// Tên chuyên khoa
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// Mô tả chuyên khoa
        /// </summary>
        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        /// <summary>
        /// Đường dẫn thân (URL slug) cho SEO
        /// </summary>
        [Column(TypeName = "ntext")]
        public string Slug { get; set; }

        /// <summary>
        /// Link ảnh chuyên khoa
        /// </summary>
        [StringLength(255)]
        public string ImageUrl { get; set; }
    }
}
