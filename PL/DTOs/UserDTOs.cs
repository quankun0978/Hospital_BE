using System;
using System.ComponentModel.DataAnnotations;

namespace Hospital_BE.PL.DTOs
{
    public class PhoneDTO
    {
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        public string Phone { get; set; }
    }

    public class CreateUserDTO
    {
        [StringLength(256, ErrorMessage = "Tên người dùng tối đa 256 ký tự")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(256, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [StringLength(255, ErrorMessage = "Họ và tên tối đa 255 ký tự")]
        public string Name { get; set; }
        [StringLength(255, ErrorMessage = "Email tối đa 255 ký tự")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mã vai trò là bắt buộc")]
        [StringLength(50, ErrorMessage = "Mã vai trò tối đa 50 ký tự")]
        public string RoleId { get; set; }
    }

    public class UpdateUserDTO
    {
        [StringLength(255, ErrorMessage = "Họ và tên tối đa 255 ký tự")]
        public string Name { get; set; }
        [StringLength(255, ErrorMessage = "Email tối đa 255 ký tự")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [StringLength(50, ErrorMessage = "Mã vai trò tối đa 50 ký tự")]
        public string RoleId { get; set; }
    }

    public class UserDTO
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }
} 