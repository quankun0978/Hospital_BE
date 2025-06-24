using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.DAL.Interfaces
{
    public interface IArticleRepository
    {
        Task<PaginatedResult<Article>> GetArticlesAsync(int pageNumber, int pageSize, string? searchTerm = null, string? category = null, string sortBy = "UpdatedAt", string sortOrder = "desc");
        Task<Article?> GetByIdAsync(Guid id);
        Task<Article?> GetBySlugAsync(string slug);
        Task<Article> CreateAsync(Article article);
        Task<Article> UpdateAsync(Article article);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<List<Article>> GetFeaturedArticlesAsync(int count = 5);
        Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null);
    }
} 