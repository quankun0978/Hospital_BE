using System.ComponentModel.DataAnnotations;

namespace Hospital_BE.PL.DTOs
{
    public class CreateArticleDto
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(255, ErrorMessage = "Tiêu đề không được vượt quá 255 ký tự")]
        public string Title { get; set; } = string.Empty;

        public string? Slug { get; set; }

        public string? Description { get; set; }

        public string? ContentHtml { get; set; }

        public string? Content { get; set; }

        [StringLength(100, ErrorMessage = "Danh mục không được vượt quá 100 ký tự")]
        public string? Category { get; set; }

        [StringLength(255, ErrorMessage = "URL ảnh không được vượt quá 255 ký tự")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Tác giả không được để trống")]
        public Guid AuthorId { get; set; }
    }

    public class UpdateArticleDto
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(255, ErrorMessage = "Tiêu đề không được vượt quá 255 ký tự")]
        public string Title { get; set; } = string.Empty;

        public string? Slug { get; set; }

        public string? Description { get; set; }

        public string? ContentHtml { get; set; }

        public string? Content { get; set; }

        [StringLength(100, ErrorMessage = "Danh mục không được vượt quá 100 ký tự")]
        public string? Category { get; set; }

        [StringLength(255, ErrorMessage = "URL ảnh không được vượt quá 255 ký tự")]
        public string? ImageUrl { get; set; }
    }

    public class ArticleDto
    {
        public Guid ArticleId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public string? ContentHtml { get; set; }
        public string? Content { get; set; }
        public string? Category { get; set; }
        public string? ImageUrl { get; set; }
        public Guid AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public DateTime PublishedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ArticleListItemDto
    {
        public Guid ArticleId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? ImageUrl { get; set; }
        public Guid AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public DateTime PublishedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 