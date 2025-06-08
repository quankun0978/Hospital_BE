using System;
using System.ComponentModel.DataAnnotations;

namespace Hospital_BE.PL.DTOs
{
    /// <summary>
    /// DTO để tạo mới specialty
    /// </summary>
    public class CreateSpecialtyDTO
    {
        [Required(ErrorMessage = "Tên chuyên khoa là bắt buộc")]
        [StringLength(255, ErrorMessage = "Tên chuyên khoa không được vượt quá 255 ký tự")]
        public string Name { get; set; }

        [StringLength(255, ErrorMessage = "URL hình ảnh không được vượt quá 255 ký tự")]
        public string ImageUrl { get; set; }

        public string Description { get; set; }

        public string Slug { get; set; }
    }

    /// <summary>
    /// DTO để cập nhật specialty
    /// </summary>
    public class UpdateSpecialtyDTO
    {
        [StringLength(255, ErrorMessage = "Tên chuyên khoa không được vượt quá 255 ký tự")]
        public string Name { get; set; }

        [StringLength(255, ErrorMessage = "URL hình ảnh không được vượt quá 255 ký tự")]
        public string ImageUrl { get; set; }

        public string Description { get; set; }

        public string Slug { get; set; }
    }

    /// <summary>
    /// DTO để hiển thị thông tin specialty
    /// </summary>
    public class SpecialtyResponseDTO
    {
        public Guid SpecialtyId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        
        // Thêm các trường để tương thích với frontend
        public string Image => ImageUrl; // Alias cho ImageUrl
        public string Link => !string.IsNullOrEmpty(Slug) ? $"/chuyen-khoa/{Slug}" : "#"; // Tạo link từ slug
    }
} 