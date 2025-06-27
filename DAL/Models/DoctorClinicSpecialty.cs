using Hospital_BE.DAL.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hospital_BE.DAL.Models
{
    public class DoctorClinicSpecialty
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public Guid ClinicId { get; set; }

        [Required]
        public Guid SpecialtyId { get; set; }

        // navigation properties
        [ForeignKey(nameof(DoctorId))]
        public virtual User Doctor { get; set; }

        [ForeignKey(nameof(ClinicId))]
        public virtual Clinic Clinic { get; set; }

        [ForeignKey(nameof(SpecialtyId))]
        public virtual Specialty Specialty { get; set; }
    }
}
