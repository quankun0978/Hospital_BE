using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.Controllers.Base;
using Hospital_BE.PL.DTOs;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleController : BaseController
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        /// <summary>
        /// Lấy danh sách tất cả bài viết có phân trang
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetArticles([FromQuery] QueryParameters parameters)
        {
            try
            {
                var result = await _articleService.GetArticlesAsync(
                    parameters.PageNumber, parameters.PageSize, parameters.SearchTerm, 
                    null, parameters.SortBy ?? "UpdatedAt", parameters.SortOrder ?? "desc");

                if (result.Success)
                {
                    var dtoResult = new PaginatedResult<ArticleListItemDto>(
                        result.Data.Data.Select(a => new ArticleListItemDto
                        {
                            ArticleId = a.ArticleId,
                            Title = a.Title,
                            Slug = a.Slug,
                            Description = a.Description,
                            Category = a.Category,
                            AuthorName = a.Author?.Name ?? "Ẩn danh",
                            PublishedAt = a.PublishedAt,
                            UpdatedAt = a.UpdatedAt
                        }).ToList(),
                        result.Data.TotalCount,
                        result.Data.CurrentPage,
                        result.Data.PageSize
                    );

                    return ApiOk(dtoResult, "Lấy danh sách bài viết thành công");
                }

                return ApiBadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return ApiError($"Lỗi khi lấy danh sách bài viết: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy bài viết theo ID (chỉ Admin)
        /// </summary>
        [HttpGet("id/{id:guid}")]
        [Authorize(Roles = "R1")]
        public async Task<IActionResult> GetArticleById(Guid id)
        {
            try
            {
                Console.WriteLine($"=== GET ARTICLE BY ID DEBUG ===");
                Console.WriteLine($"Article ID: {id}");
                
                var result = await _articleService.GetArticleByIdAsync(id);
                
                Console.WriteLine($"Service result success: {result.Success}");
                Console.WriteLine($"Service result message: {result.Message}");
                Console.WriteLine($"Service result data is null: {result.Data == null}");
                
                if (result.Success && result.Data != null)
                {
                    var dto = new ArticleDto
                    {
                        ArticleId = result.Data.ArticleId,
                        Title = result.Data.Title,
                        Slug = result.Data.Slug,
                        Description = result.Data.Description,
                        Content = result.Data.Content,
                        ContentHtml = result.Data.ContentHtml,
                        Category = result.Data.Category,
                        AuthorId = result.Data.AuthorId,
                        AuthorName = result.Data.Author?.Name ?? "Ẩn danh",
                        PublishedAt = result.Data.PublishedAt,
                        UpdatedAt = result.Data.UpdatedAt
                    };

                    Console.WriteLine($"DTO created successfully for article: {dto.Title}");
                    Console.WriteLine("===============================");
                    return ApiOk(dto, "Lấy bài viết thành công");
                }

                Console.WriteLine("===============================");
                return ApiNotFound<ArticleDto>(result.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetArticleById: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Console.WriteLine("===============================");
                return ApiError($"Lỗi khi lấy bài viết: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy bài viết theo slug
        /// </summary>
        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetArticleBySlug(string slug)
        {
            try
            {
                var result = await _articleService.GetArticleBySlugAsync(slug);
                if (result.Success)
                {
                    var dto = new ArticleDto
                    {
                        ArticleId = result.Data.ArticleId,
                        Title = result.Data.Title,
                        Slug = result.Data.Slug,
                        Description = result.Data.Description,
                        Content = result.Data.Content,
                        ContentHtml = result.Data.ContentHtml,
                        Category = result.Data.Category,
                        AuthorId = result.Data.AuthorId,
                        AuthorName = result.Data.Author?.Name ?? "Ẩn danh",
                        PublishedAt = result.Data.PublishedAt,
                        UpdatedAt = result.Data.UpdatedAt
                    };

                    return ApiOk(dto, "Lấy bài viết thành công");
                }

                return ApiNotFound<ArticleDto>(result.Message);
            }
            catch (Exception ex)
            {
                return ApiError($"Lỗi khi lấy bài viết: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy bài viết theo danh mục
        /// </summary>
        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetArticlesByCategory(string category, [FromQuery] QueryParameters parameters)
        {
            try
            {
                var result = await _articleService.GetArticlesAsync(
                    parameters.PageNumber, parameters.PageSize, parameters.SearchTerm, 
                    category, parameters.SortBy ?? "UpdatedAt", parameters.SortOrder ?? "desc");

                if (result.Success)
                {
                    var dtoResult = result.Data.Data.Select(a => new ArticleListItemDto
                    {
                        ArticleId = a.ArticleId,
                        Title = a.Title,
                        Slug = a.Slug,
                        Description = a.Description,
                        Category = a.Category,
                        AuthorName = a.Author?.Name ?? "Ẩn danh",
                        PublishedAt = a.PublishedAt,
                        UpdatedAt = a.UpdatedAt
                    }).ToList();

                    return ApiOk(dtoResult, "Lấy bài viết theo danh mục thành công");
                }

                return ApiBadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return ApiError($"Lỗi khi lấy bài viết theo danh mục: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy bài viết nổi bật
        /// </summary>
        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedArticles([FromQuery] int count = 5)
        {
            try
            {
                var result = await _articleService.GetFeaturedArticlesAsync(count);
                if (result.Success)
                {
                    var dtoResult = result.Data.Select(a => new ArticleListItemDto
                    {
                        ArticleId = a.ArticleId,
                        Title = a.Title,
                        Slug = a.Slug,
                        Description = a.Description,
                        Category = a.Category,
                        AuthorName = a.Author?.Name ?? "Ẩn danh",
                        PublishedAt = a.PublishedAt,
                        UpdatedAt = a.UpdatedAt
                    }).ToList();

                    return ApiOk(dtoResult, "Lấy bài viết nổi bật thành công");
                }

                return ApiBadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return ApiError($"Lỗi khi lấy bài viết nổi bật: {ex.Message}");
            }
        }

        /// <summary>
        /// Tạo bài viết mới (chỉ Admin)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "R1")]
        public async Task<IActionResult> CreateArticle([FromBody] CreateArticleDto createDto)
        {
            try
            {
                // Debug logging
                Console.WriteLine("=== CREATE ARTICLE DEBUG ===");
                Console.WriteLine($"Title: {createDto.Title}");
                Console.WriteLine($"ContentHtml length: {createDto.ContentHtml?.Length ?? 0}");
                Console.WriteLine($"Has Vietnamese chars: {(createDto.ContentHtml != null && System.Text.RegularExpressions.Regex.IsMatch(createDto.ContentHtml, @"[àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ]", System.Text.RegularExpressions.RegexOptions.IgnoreCase))}");
                Console.WriteLine("============================");

                if (!ModelState.IsValid)
                {
                    return ApiBadRequest("Dữ liệu không hợp lệ");
                }

                var authorId = GetCurrentUserIdAsGuid();
                var result = await _articleService.CreateArticleAsync(createDto, authorId);

                if (result.Success)
                {
                    var dto = new ArticleDto
                    {
                        ArticleId = result.Data.ArticleId,
                        Title = result.Data.Title,
                        Slug = result.Data.Slug,
                        Description = result.Data.Description,
                        Content = result.Data.Content,
                        ContentHtml = result.Data.ContentHtml,
                        Category = result.Data.Category,
                        AuthorId = result.Data.AuthorId,
                        AuthorName = result.Data.Author?.Name ?? "Ẩn danh",
                        PublishedAt = result.Data.PublishedAt,
                        UpdatedAt = result.Data.UpdatedAt
                    };

                    return ApiOk(dto, "Tạo bài viết thành công");
                }

                return ApiBadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return ApiError($"Lỗi khi tạo bài viết: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật bài viết (chỉ Admin)
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "R1")]
        public async Task<IActionResult> UpdateArticle(Guid id, [FromBody] UpdateArticleDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ApiBadRequest("Dữ liệu không hợp lệ");
                }

                var authorId = GetCurrentUserIdAsGuid();
                var result = await _articleService.UpdateArticleAsync(id, updateDto, authorId);

                if (result.Success)
                {
                    var dto = new ArticleDto
                    {
                        ArticleId = result.Data.ArticleId,
                        Title = result.Data.Title,
                        Slug = result.Data.Slug,
                        Description = result.Data.Description,
                        Content = result.Data.Content,
                        ContentHtml = result.Data.ContentHtml,
                        Category = result.Data.Category,
                        AuthorId = result.Data.AuthorId,
                        AuthorName = result.Data.Author?.Name ?? "Ẩn danh",
                        PublishedAt = result.Data.PublishedAt,
                        UpdatedAt = result.Data.UpdatedAt
                    };

                    return ApiOk(dto, "Cập nhật bài viết thành công");
                }

                return ApiBadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return ApiError($"Lỗi khi cập nhật bài viết: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa bài viết (chỉ Admin)
        /// </summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "R1")]
        public async Task<IActionResult> DeleteArticle(Guid id)
        {
            try
            {
                var authorId = GetCurrentUserIdAsGuid();
                var result = await _articleService.DeleteArticleAsync(id, authorId);

                if (result.Success)
                {
                    return ApiOk<object>(null, "Xóa bài viết thành công");
                }

                return ApiBadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return ApiError($"Lỗi khi xóa bài viết: {ex.Message}");
            }
        }

        /// <summary>
        /// Tạo slug từ tiêu đề (chỉ Admin)
        /// </summary>
        [HttpPost("generate-slug")]
        [Authorize(Roles = "R1")]
        public async Task<IActionResult> GenerateSlug([FromBody] GenerateSlugRequest request)
        {
            try
            {
                var result = await _articleService.GenerateUniqueSlugAsync(request.Title);
                if (result.Success)
                {
                    return ApiOk(new { slug = result.Data }, "Tạo slug thành công");
                }

                return ApiBadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return ApiError($"Lỗi khi tạo slug: {ex.Message}");
            }
        }

        private IActionResult ApiError(string message)
        {
            return StatusCode(500, ApiResponseHelper.Error<object>(message));
        }
    }

    public class GenerateSlugRequest
    {
        public string Title { get; set; } = string.Empty;
    }
} 