 using System.Text;
using System.Text.RegularExpressions;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.BLL.Models;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.BLL.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;

        public ArticleService(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task<ServiceResult<PaginatedResult<Article>>> GetArticlesAsync(
            int pageNumber, int pageSize, string? searchTerm, string? category,
            string sortBy = "UpdatedAt", string sortOrder = "desc")
        {
            try
            {
                var articles = await _articleRepository.GetArticlesAsync(
                    pageNumber, pageSize, searchTerm, category, sortBy, sortOrder);
                return ServiceResult<PaginatedResult<Article>>.Ok("Lấy danh sách bài viết thành công", articles);
            }
            catch (Exception ex)
            {
                return ServiceResult<PaginatedResult<Article>>.Error($"Lỗi khi lấy danh sách bài viết: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Article>> GetArticleByIdAsync(Guid id)
        {
            try
            {
                var article = await _articleRepository.GetByIdAsync(id);
                if (article == null)
                {
                    return ServiceResult<Article>.Error("Không tìm thấy bài viết");
                }
                return ServiceResult<Article>.Ok("Lấy bài viết thành công", article);
            }
            catch (Exception ex)
            {
                return ServiceResult<Article>.Error($"Lỗi khi lấy bài viết: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Article>> GetArticleBySlugAsync(string slug)
        {
            try
            {
                var article = await _articleRepository.GetBySlugAsync(slug);
                if (article == null)
                {
                    return ServiceResult<Article>.Error("Không tìm thấy bài viết");
                }
                return ServiceResult<Article>.Ok("Lấy bài viết thành công", article);
            }
            catch (Exception ex)
            {
                return ServiceResult<Article>.Error($"Lỗi khi lấy bài viết: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Article>> CreateArticleAsync(CreateArticleDto createDto, Guid authorId)
        {
            try
            {
                if (string.IsNullOrEmpty(createDto.Title))
                {
                    return ServiceResult<Article>.Error("Tiêu đề không được để trống");
                }

                if (string.IsNullOrEmpty(createDto.Content))
                {
                    return ServiceResult<Article>.Error("Nội dung không được để trống");
                }

                // Kiểm tra tác giả có tồn tại không
                if (authorId == Guid.Empty)
                {
                    return ServiceResult<Article>.Error("Tác giả không hợp lệ");
                }

                // Tạo slug duy nhất
                var slug = await GenerateUniqueSlugAsync(createDto.Title);
                if (!slug.Success)
                {
                    return ServiceResult<Article>.Error(slug.Message);
                }

                var article = new Article
                {
                    ArticleId = Guid.NewGuid(),
                    Title = createDto.Title,
                    Slug = slug.Data,
                    Description = createDto.Description,
                    ContentHtml = createDto.ContentHtml,
                    Content = createDto.Content,
                    Category = createDto.Category,
                    AuthorId = authorId,
                    PublishedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var createdArticle = await _articleRepository.CreateAsync(article);
                return ServiceResult<Article>.Ok("Tạo bài viết thành công", createdArticle);
            }
            catch (Exception ex)
            {
                return ServiceResult<Article>.Error($"Lỗi khi tạo bài viết: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Article>> UpdateArticleAsync(Guid id, UpdateArticleDto updateDto, Guid authorId)
        {
            try
            {
                var existingArticle = await _articleRepository.GetByIdAsync(id);
                if (existingArticle == null)
                {
                    return ServiceResult<Article>.Error("Bài viết không tồn tại");
                }

                // Kiểm tra quyền sở hữu - chỉ tác giả hoặc admin mới có thể sửa
                if (existingArticle.AuthorId != authorId)
                {
                    return ServiceResult<Article>.Error("Bạn không có quyền sửa bài viết này");
                }

                // Cập nhật thông tin
                existingArticle.Title = updateDto.Title;
                existingArticle.Description = updateDto.Description;
                existingArticle.ContentHtml = updateDto.ContentHtml;
                existingArticle.Content = updateDto.Content;
                existingArticle.Category = updateDto.Category;
                existingArticle.UpdatedAt = DateTime.Now;

                // Tạo slug mới nếu title thay đổi
                if (!string.IsNullOrEmpty(updateDto.Slug))
                {
                    existingArticle.Slug = updateDto.Slug;
                }
                else
                {
                    var newSlug = await GenerateUniqueSlugAsync(updateDto.Title);
                    if (newSlug.Success)
                    {
                        existingArticle.Slug = newSlug.Data;
                    }
                }

                var updatedArticle = await _articleRepository.UpdateAsync(existingArticle);
                return ServiceResult<Article>.Ok("Cập nhật bài viết thành công", updatedArticle);
            }
            catch (Exception ex)
            {
                return ServiceResult<Article>.Error($"Lỗi khi cập nhật bài viết: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteArticleAsync(Guid id, Guid authorId)
        {
            try
            {
                var existingArticle = await _articleRepository.GetByIdAsync(id);
                if (existingArticle == null)
                {
                    return ServiceResult<bool>.Error("Bài viết không tồn tại");
                }

                // Kiểm tra quyền sở hữu - chỉ tác giả hoặc admin mới có thể xóa
                if (existingArticle.AuthorId != authorId)
                {
                    return ServiceResult<bool>.Error("Bạn không có quyền xóa bài viết này");
                }

                var result = await _articleRepository.DeleteAsync(id);
                return ServiceResult<bool>.Ok("Xóa bài viết thành công", result);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Error($"Lỗi khi xóa bài viết: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ExistsAsync(Guid id)
        {
            try
            {
                var exists = await _articleRepository.ExistsAsync(id);
                return ServiceResult<bool>.Ok("Kiểm tra bài viết thành công", exists);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Error($"Lỗi khi kiểm tra bài viết: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<Article>>> GetFeaturedArticlesAsync(int count = 5)
        {
            try
            {
                var articles = await _articleRepository.GetFeaturedArticlesAsync(count);
                return ServiceResult<List<Article>>.Ok("Lấy bài viết nổi bật thành công", articles);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<Article>>.Error($"Lỗi khi lấy bài viết nổi bật: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> GenerateUniqueSlugAsync(string title, Guid? excludeId = null)
        {
            try
            {
                var baseSlug = GenerateSlugFromTitle(title);
                var slug = baseSlug;
                var counter = 1;

                while (await _articleRepository.SlugExistsAsync(slug, excludeId))
                {
                    slug = $"{baseSlug}-{counter}";
                    counter++;
                }

                return ServiceResult<string>.Ok("Tạo slug thành công", slug);
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Error($"Lỗi khi tạo slug: {ex.Message}");
            }
        }

        private string GenerateSlugFromTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
                return string.Empty;

            // Chuyển về chữ thường và loại bỏ dấu tiếng Việt
            var slug = title.ToLowerInvariant();
            
            // Dictionary để chuyển ký tự có dấu thành không dấu
            var vietnameseChars = new Dictionary<char, char>
            {
                {'à', 'a'}, {'á', 'a'}, {'ạ', 'a'}, {'ả', 'a'}, {'ã', 'a'},
                {'â', 'a'}, {'ầ', 'a'}, {'ấ', 'a'}, {'ậ', 'a'}, {'ẩ', 'a'}, {'ẫ', 'a'},
                {'ă', 'a'}, {'ằ', 'a'}, {'ắ', 'a'}, {'ặ', 'a'}, {'ẳ', 'a'}, {'ẵ', 'a'},
                {'è', 'e'}, {'é', 'e'}, {'ẹ', 'e'}, {'ẻ', 'e'}, {'ẽ', 'e'},
                {'ê', 'e'}, {'ề', 'e'}, {'ế', 'e'}, {'ệ', 'e'}, {'ể', 'e'}, {'ễ', 'e'},
                {'ì', 'i'}, {'í', 'i'}, {'ị', 'i'}, {'ỉ', 'i'}, {'ĩ', 'i'},
                {'ò', 'o'}, {'ó', 'o'}, {'ọ', 'o'}, {'ỏ', 'o'}, {'õ', 'o'},
                {'ô', 'o'}, {'ồ', 'o'}, {'ố', 'o'}, {'ộ', 'o'}, {'ổ', 'o'}, {'ỗ', 'o'},
                {'ơ', 'o'}, {'ờ', 'o'}, {'ớ', 'o'}, {'ợ', 'o'}, {'ở', 'o'}, {'ỡ', 'o'},
                {'ù', 'u'}, {'ú', 'u'}, {'ụ', 'u'}, {'ủ', 'u'}, {'ũ', 'u'},
                {'ư', 'u'}, {'ừ', 'u'}, {'ứ', 'u'}, {'ự', 'u'}, {'ử', 'u'}, {'ữ', 'u'},
                {'ỳ', 'y'}, {'ý', 'y'}, {'ỵ', 'y'}, {'ỷ', 'y'}, {'ỹ', 'y'},
                {'đ', 'd'}
            };

            var result = new StringBuilder();
            foreach (var c in slug)
            {
                if (vietnameseChars.ContainsKey(c))
                {
                    result.Append(vietnameseChars[c]);
                }
                else
                {
                    result.Append(c);
                }
            }

            slug = result.ToString();

            // Thay thế các ký tự không hợp lệ bằng dấu gạch ngang
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-");
            slug = Regex.Replace(slug, @"-+", "-");
            slug = slug.Trim('-');

            return slug;
        }
    }
} 