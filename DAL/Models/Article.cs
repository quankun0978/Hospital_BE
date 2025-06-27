using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_BE.DAL.Models
{
    [Table("Articles")]
    public class Article
    {
        [Key]
        public Guid ArticleId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Slug { get; set; }

        [Column(TypeName = "NVARCHAR(MAX)")]
        public string? Description { get; set; }

        [Column(TypeName = "NVARCHAR(MAX)")]
        public string? ContentHtml { get; set; }

        [Column(TypeName = "NVARCHAR(MAX)")]
        public string? Content { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        [Required]
        public Guid AuthorId { get; set; }

        [Required]
        public DateTime PublishedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("AuthorId")]
        public virtual User? Author { get; set; }
    }
} 