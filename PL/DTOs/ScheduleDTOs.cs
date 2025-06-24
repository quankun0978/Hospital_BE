using System.ComponentModel.DataAnnotations;

namespace Hospital_BE.PL.DTOs
{
    public class CreateScheduleDto
    {
        [Required(ErrorMessage = "Ngày khám là bắt buộc")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Thời gian khám là bắt buộc")]
        public string TimeType { get; set; }

        [Required(ErrorMessage = "Bác sĩ là bắt buộc")]
        public Guid DoctorId { get; set; }
        
        public bool IsActive { get; set; } = true;
    }

    public class UpdateScheduleDto
    {
        [Required(ErrorMessage = "Ngày khám là bắt buộc")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Thời gian khám là bắt buộc")]
        public string TimeType { get; set; }

        [Required(ErrorMessage = "Bác sĩ là bắt buộc")]
        public Guid DoctorId { get; set; }
        
        public bool IsActive { get; set; } = true;
    }

    public class ScheduleResponseDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string TimeType { get; set; }
        public string TimeTypeText { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorEmail { get; set; }
        public bool IsActive { get; set; }
    }

    public class DoctorOptionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
} 