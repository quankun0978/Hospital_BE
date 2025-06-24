using Microsoft.EntityFrameworkCore;
using Hospital_BE.DAL.Context;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.DAL.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly ApplicationDbContext _context;

        public ArticleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<Article>> GetArticlesAsync(int pageNumber, int pageSize, string? searchTerm = null, string? category = null, string sortBy = "UpdatedAt", string sortOrder = "desc")
        {
            var query = _context.Articles
                .Include(a => a.Author)
                .AsQueryable();

            // Search filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a => a.Title.Contains(searchTerm) || 
                                        (a.Description != null && a.Description.Contains(searchTerm)));
            }

            // Category filter
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(a => a.Category == category);
            }

            // Sorting
            query = sortBy.ToLower() switch
            {
                "title" => sortOrder.ToLower() == "asc" 
                    ? query.OrderBy(a => a.Title) 
                    : query.OrderByDescending(a => a.Title),
                "publishedat" => sortOrder.ToLower() == "asc" 
                    ? query.OrderBy(a => a.PublishedAt) 
                    : query.OrderByDescending(a => a.PublishedAt),
                _ => sortOrder.ToLower() == "asc" 
                    ? query.OrderBy(a => a.UpdatedAt) 
                    : query.OrderByDescending(a => a.UpdatedAt)
            };

            var totalRecords = await query.CountAsync();

            var articles = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PaginatedResult<Article>(
                articles,
                totalRecords,
                pageNumber,
                pageSize
            );

            return result;
        }

        public async Task<Article?> GetByIdAsync(Guid id)
        {
            return await _context.Articles
                .Include(a => a.Author)
                .FirstOrDefaultAsync(a => a.ArticleId == id);
        }

        public async Task<Article?> GetBySlugAsync(string slug)
        {
            return await _context.Articles
                .Include(a => a.Author)
                .FirstOrDefaultAsync(a => a.Slug == slug);
        }

        public async Task<Article> CreateAsync(Article article)
        {
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
            return article;
        }

        public async Task<Article> UpdateAsync(Article article)
        {
            _context.Articles.Update(article);
            await _context.SaveChangesAsync();
            return article;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var article = await GetByIdAsync(id);
            if (article == null) return false;

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Articles.AnyAsync(a => a.ArticleId == id);
        }

        public async Task<List<Article>> GetFeaturedArticlesAsync(int count = 5)
        {
            return await _context.Articles
                .Include(a => a.Author)
                .OrderByDescending(a => a.PublishedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null)
        {
            var query = _context.Articles.Where(a => a.Slug == slug);
            
            if (excludeId.HasValue)
            {
                query = query.Where(a => a.ArticleId != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }
    }
} 