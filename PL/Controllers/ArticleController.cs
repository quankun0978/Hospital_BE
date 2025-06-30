using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.Controllers.Base;
using Hospital_BE.PL.DTOs;
using Hospital_BE.PL.DTOs.Common;
using System.IO;

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
                            ImageUrl = a.ImageUrl,
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
                        ImageUrl = result.Data.ImageUrl,
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
                        ImageUrl = result.Data.ImageUrl,
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
                        ImageUrl = a.ImageUrl,
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
                        ImageUrl = a.ImageUrl,
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
                Console.WriteLine("=== CREATE ARTICLE CONTROLLER DEBUG ===");
                Console.WriteLine($"Received DTO - Title: {createDto.Title}");
                Console.WriteLine($"Received DTO - Description: {createDto.Description?.Substring(0, Math.Min(100, createDto.Description?.Length ?? 0))}...");
                Console.WriteLine($"Received DTO - Content length: {createDto.Content?.Length ?? 0}");
                Console.WriteLine($"Received DTO - ContentHtml length: {createDto.ContentHtml?.Length ?? 0}");
                Console.WriteLine($"Content has Vietnamese: {(createDto.Content != null && System.Text.RegularExpressions.Regex.IsMatch(createDto.Content, @"[àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ]", System.Text.RegularExpressions.RegexOptions.IgnoreCase))}");
                Console.WriteLine($"ContentHtml has Vietnamese: {(createDto.ContentHtml != null && System.Text.RegularExpressions.Regex.IsMatch(createDto.ContentHtml, @"[àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ]", System.Text.RegularExpressions.RegexOptions.IgnoreCase))}");
                Console.WriteLine($"Content preview: {createDto.Content?.Substring(0, Math.Min(200, createDto.Content?.Length ?? 0))}...");
                Console.WriteLine($"ContentHtml preview: {createDto.ContentHtml?.Substring(0, Math.Min(200, createDto.ContentHtml?.Length ?? 0))}...");

                var currentUserId = GetCurrentUserIdAsGuid();
                if (currentUserId == Guid.Empty)
                {
                    return Unauthorized(ApiResponseHelper.Error<ArticleDto>("Không thể xác định người dùng"));
                }

                var createArticleDto = new Hospital_BE.PL.DTOs.CreateArticleDto
                {
                    Title = createDto.Title,
                    Slug = createDto.Slug,
                    Description = createDto.Description,
                    Content = createDto.Content,
                    ContentHtml = createDto.ContentHtml,
                    Category = createDto.Category,
                    ImageUrl = createDto.ImageUrl
                };

                Console.WriteLine("=== BEFORE SERVICE CALL ===");
                Console.WriteLine($"Service DTO - Content: {createArticleDto.Content?.Substring(0, Math.Min(100, createArticleDto.Content?.Length ?? 0))}...");
                Console.WriteLine($"Service DTO - ContentHtml: {createArticleDto.ContentHtml?.Substring(0, Math.Min(100, createArticleDto.ContentHtml?.Length ?? 0))}...");

                var result = await _articleService.CreateArticleAsync(createArticleDto, currentUserId);
                
                Console.WriteLine($"Service result success: {result.Success}");
                Console.WriteLine($"Service result message: {result.Message}");
                Console.WriteLine("===============================");

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
                        ImageUrl = result.Data.ImageUrl,
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
                Console.WriteLine($"Exception in CreateArticle: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
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
                Console.WriteLine("=== UPDATE ARTICLE CONTROLLER DEBUG ===");
                Console.WriteLine($"Article ID: {id}");
                Console.WriteLine($"Update DTO - Title: {updateDto.Title}");
                Console.WriteLine($"Update DTO - Description: {updateDto.Description?.Substring(0, Math.Min(100, updateDto.Description?.Length ?? 0))}...");
                Console.WriteLine($"Update DTO - Content length: {updateDto.Content?.Length ?? 0}");
                Console.WriteLine($"Update DTO - ContentHtml length: {updateDto.ContentHtml?.Length ?? 0}");
                Console.WriteLine($"Content has Vietnamese: {(updateDto.Content != null && System.Text.RegularExpressions.Regex.IsMatch(updateDto.Content, @"[àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ]", System.Text.RegularExpressions.RegexOptions.IgnoreCase))}");
                Console.WriteLine($"ContentHtml has Vietnamese: {(updateDto.ContentHtml != null && System.Text.RegularExpressions.Regex.IsMatch(updateDto.ContentHtml, @"[àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ]", System.Text.RegularExpressions.RegexOptions.IgnoreCase))}");

                var currentUserId = GetCurrentUserIdAsGuid();
                if (currentUserId == Guid.Empty)
                {
                    return Unauthorized(ApiResponseHelper.Error<ArticleDto>("Không thể xác định người dùng"));
                }

                var updateArticleDto = new Hospital_BE.PL.DTOs.UpdateArticleDto
                {
                    Title = updateDto.Title,
                    Slug = updateDto.Slug,
                    Description = updateDto.Description,
                    Content = updateDto.Content,
                    ContentHtml = updateDto.ContentHtml,
                    Category = updateDto.Category,
                    ImageUrl = updateDto.ImageUrl
                };

                var result = await _articleService.UpdateArticleAsync(id, updateArticleDto, currentUserId);
                
                Console.WriteLine($"Update service result success: {result.Success}");
                Console.WriteLine($"Update service result message: {result.Message}");
                Console.WriteLine("===============================");

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
                        ImageUrl = result.Data.ImageUrl,
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
                Console.WriteLine($"Exception in UpdateArticle: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
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

        /// <summary>
        /// Upload ảnh cho bài viết (chỉ Admin)
        /// </summary>
        [HttpPost("upload-image")]
        [Authorize(Roles = "R1")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return ApiBadRequest("Vui lòng chọn file ảnh");
                }

                // Kiểm tra định dạng file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return ApiBadRequest("Chỉ cho phép tải lên các file ảnh: " + string.Join(", ", allowedExtensions));
                }

                // Kiểm tra kích thước file (tối đa 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return ApiBadRequest("Kích thước file không được vượt quá 5MB");
                }

                // Tạo tên file duy nhất
                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var uploadPath = Path.Combine("PL", "static", "image", "article");
                var fullPath = Path.Combine(uploadPath, fileName);

                // Tạo thư mục nếu chưa tồn tại
                Directory.CreateDirectory(uploadPath);

                // Lưu file
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Trả về đường dẫn tương đối
                var relativePath = $"/static/image/article/{fileName}";
                
                return ApiOk(new { imageUrl = relativePath }, "Tải ảnh lên thành công");
            }
            catch (Exception ex)
            {
                return ApiError($"Lỗi khi tải ảnh lên: {ex.Message}");
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