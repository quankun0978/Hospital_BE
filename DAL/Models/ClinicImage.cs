using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_BE.DAL.Models
{
    [Table("ClinicImages")]
    public class ClinicImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "text")]
        public string ImageFallbackUrl { get; set; }

        [Column(TypeName = "bit")]
        public bool IsBackground { get; set; }

        [Required]
        public Guid ClinicId { get; set; }

        // Navigation property
        [ForeignKey(nameof(ClinicId))]
        public virtual Clinic Clinic { get; set; }
    }
}
