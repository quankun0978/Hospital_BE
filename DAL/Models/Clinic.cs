using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_BE.DAL.Models
{
    /// <summary>
    /// Bảng thông tin về phòng khám/cơ sở y tế
    /// </summary>
    [Table("Clinics")]
    public class Clinic
    {
        /// <summary>
        /// Khóa chính của phòng khám, là định danh duy nhất
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ClinicId { get; set; }

        /// <summary>
        /// Tên phòng/cơ sở y tế
        /// </summary>
        [StringLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// Địa chỉ cơ sở y tế
        /// </summary>
        [Column(TypeName = "ntext")]
        public string Address { get; set; }

        /// <summary>
        /// Mô tả cơ sở y tế
        /// </summary>
        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        /// <summary>
        /// Đường dẫn thân (URL slug) cho SEO
        /// </summary>
        [Column(TypeName = "ntext")]
        public string Slug { get; set; }

        /// <summary>
        /// Link ảnh cơ sở y tế
        /// </summary>
        [StringLength(255)]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Link ảnh logo của cơ sở y tế
        /// </summary>
        [StringLength(255)]
        public string LogoImg { get; set; }

        /// <summary>
        /// Đánh dấu có phải là bệnh viện hay không
        /// True: Là bệnh viện, False: Là phòng khám
        /// </summary>
        [Column(TypeName = "bit")]
        public bool IsHospital { get; set; }

        /// <summary>
        /// Danh sách các bác sĩ làm việc tại phòng khám này
        /// </summary>
        public virtual ICollection<DoctorInfo> Doctors { get; set; }
    }
} 