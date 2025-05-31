using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_BE.DAL.Models
{
    /// <summary>
    /// Bảng thông tin chi tiết của bác sĩ
    /// </summary>
    [Table("DoctorInfos")]
    public class DoctorInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Khóa ngoại tham chiếu đến UserId trong bảng Users
        /// </summary>
        [Required]
        public Guid DoctorId { get; set; }

        // Navigation property: một DoctorInfo thuộc về một User (Doctor)
        [ForeignKey(nameof(DoctorId))]
        public virtual User Doctor { get; set; }

        /// <summary>
        /// Khóa ngoại tham chiếu đến CodeKey trong bảng Allcodes (cho loại PRICE)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string PriceId { get; set; }

        // Navigation property: một DoctorInfo có một Price
        [ForeignKey(nameof(PriceId))]
        public virtual Allcode Price { get; set; }

        /// <summary>
        /// Khóa ngoại tham chiếu đến CodeKey trong bảng Allcodes (cho loại POSITION)
        /// </summary>
        [StringLength(50)]
        public string PositionId { get; set; }

        // Navigation property: một DoctorInfo có một Position
        [ForeignKey(nameof(PositionId))]
        public virtual Allcode Position { get; set; }

        /// <summary>
        /// Khóa ngoại tham chiếu đến ClinicId trong bảng Clinics
        /// </summary>
        public Guid? ClinicId { get; set; }

        // Navigation property: một DoctorInfo thuộc về một Clinic
        [ForeignKey(nameof(ClinicId))]
        public virtual Clinic Clinic { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string Slug { get; set; }

        [Column(TypeName = "ntext")]
        public string Note { get; set; }

        /// <summary>
        /// Link ảnh của bác sĩ
        /// </summary>
        [StringLength(255)]
        public string ImageUrl { get; set; }

        public int? Count { get; set; }
    }
} 