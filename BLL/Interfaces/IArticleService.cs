using Hospital_BE.DAL.Models;
using Hospital_BE.BLL.Models;
using Hospital_BE.PL.DTOs;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.BLL.Interfaces
{
    public interface IArticleService
    {
        Task<ServiceResult<PaginatedResult<Article>>> GetArticlesAsync(int pageNumber, int pageSize, string? searchTerm, string? category, string sortBy = "UpdatedAt", string sortOrder = "desc");
        Task<ServiceResult<Article>> GetArticleByIdAsync(Guid id);
        Task<ServiceResult<Article>> GetArticleBySlugAsync(string slug);
        Task<ServiceResult<Article>> CreateArticleAsync(CreateArticleDto createDto, Guid authorId);
        Task<ServiceResult<Article>> UpdateArticleAsync(Guid id, UpdateArticleDto updateDto, Guid authorId);
        Task<ServiceResult<bool>> DeleteArticleAsync(Guid id, Guid authorId);
        Task<ServiceResult<bool>> ExistsAsync(Guid id);
        Task<ServiceResult<List<Article>>> GetFeaturedArticlesAsync(int count = 5);
        Task<ServiceResult<string>> GenerateUniqueSlugAsync(string title, Guid? excludeId = null);
    }
} 