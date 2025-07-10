using System;
using System.ComponentModel.DataAnnotations;

namespace Hospital_BE.PL.DTOs
{
    public class UserProfileDTO
    {
        [Required(ErrorMessage = "Tên đầy đủ là bắt buộc.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Ngày sinh là bắt buộc.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        public string Address { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Giới tính là bắt buộc.")]
        public string Gender { get; set; }
    }
} 