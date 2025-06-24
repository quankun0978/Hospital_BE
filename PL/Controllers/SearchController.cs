using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Meilisearch;
using Hospital_BE.DAL.Context;
using Microsoft.EntityFrameworkCore;

namespace Hospital_BE.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly MeilisearchClient _meiliSearchClient;
        private readonly ApplicationDbContext _context;
        private const string IndexName = "hospital_search";

        public SearchController(MeilisearchClient meiliSearchClient, ApplicationDbContext context)
        {
            _meiliSearchClient = meiliSearchClient;
            _context = context;
        }

        [HttpGet("global")]
        public async Task<IActionResult> GlobalSearch(
            [FromQuery] string q = "",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12,
            [FromQuery] string type = "all",
            [FromQuery] string specialty = "",
            [FromQuery] string location = "",
            [FromQuery] string sortBy = "relevance",
            [FromQuery] string sortOrder = "desc",
            [FromQuery] double minRating = 0,
            [FromQuery] decimal minPrice = 0,
            [FromQuery] decimal maxPrice = 0,
            [FromQuery] bool onlineOnly = false)
        {
            try
            {
                var allResults = new List<SearchResultDto>();

                // Lấy doctors từ database thông qua DoctorInfo
                if (type == "all" || type == "doctor")
                {
                    var doctorInfosQuery = _context.DoctorInfos
                        .Include(di => di.Doctor)
                        .Include(di => di.Position)
                        .Include(di => di.Price)
                        .Include(di => di.Clinic)
                        .AsQueryable();

                    if (!string.IsNullOrEmpty(q))
                    {
                        doctorInfosQuery = doctorInfosQuery.Where(di => 
                            di.Doctor.Name.Contains(q) ||
                            (di.Clinic != null && di.Clinic.Address != null && di.Clinic.Address.Contains(q)) ||
                            (di.Clinic != null && di.Clinic.Name != null && di.Clinic.Name.Contains(q))
                        );
                    }

                    if (!string.IsNullOrEmpty(specialty))
                    {
                        var specialtyDoctorIds = await _context.DoctorClinicSpecialties
                            .Where(dcs => dcs.Specialty.Name.Contains(specialty))
                            .Select(dcs => dcs.DoctorId)
                            .ToListAsync();

                        doctorInfosQuery = doctorInfosQuery.Where(di =>
                            specialtyDoctorIds.Contains(di.DoctorId)
                        );
                    }

                    // Price filter - Sử dụng chuỗi từ Allcode thay vì decimal
                    // Tạm thời bỏ price filter vì cần mapping price codes

                    var doctorInfos = await doctorInfosQuery.ToListAsync();

                    var doctorResults = doctorInfos.Select(di => new SearchResultDto
                    {
                        Id = di.Id,
                        Type = "doctor",
                        Name = di.Doctor?.Name ?? "",
                        Title = di.PositionId == "P1" ? "Giáo sư" : 
                               di.PositionId == "P2" ? "Phó giáo sư" :
                               di.PositionId == "P3" ? "Tiến sĩ" : 
                               di.PositionId == "P4" ? "Thạc sĩ" : "Bác sĩ",
                        Specialties = GetDoctorSpecialties(di.DoctorId),
                        Address = di.Clinic?.Address ?? "",
                        Image = di.ImageUrl ?? "",
                        Rating = 4.5, // Tạm thời hardcode, sau này có thể tính từ reviews
                        ReviewCount = 100, // Tạm thời hardcode
                        IsOnline = true, // Tạm thời hardcode
                        Price = 200000, // Tạm thời hardcode, cần mapping từ Price code
                        Slug = di.Slug ?? CreateSlug(di.Doctor?.Name ?? ""),
                        Highlights = CreateHighlights(di.Doctor?.Name ?? "", di.Clinic?.Address ?? "", q)
                    }).ToList();

                    // Filter by online only
                    if (onlineOnly)
                    {
                        doctorResults = doctorResults.Where(d => d.IsOnline).ToList();
                    }

                    // Filter by rating
                    if (minRating > 0)
                    {
                        doctorResults = doctorResults.Where(d => d.Rating >= minRating).ToList();
                    }

                    allResults.AddRange(doctorResults);
                }

                // Lấy clinics từ database (chỉ phòng khám, không phải bệnh viện)
                if (type == "all" || type == "clinic")
                {
                    var clinicsQuery = _context.Clinics.Where(c => !c.IsHospital).AsQueryable();

                    if (!string.IsNullOrEmpty(q))
                    {
                        clinicsQuery = clinicsQuery.Where(c => 
                            c.Name.Contains(q) ||
                            (c.Address != null && c.Address.Contains(q))
                        );
                    }

                    if (!string.IsNullOrEmpty(specialty))
                    {
                        var specialtyClinicIds = await _context.DoctorClinicSpecialties
                            .Where(dcs => dcs.Specialty.Name.Contains(specialty))
                            .Select(dcs => dcs.ClinicId)
                            .Distinct()
                            .ToListAsync();

                        clinicsQuery = clinicsQuery.Where(c =>
                            specialtyClinicIds.Contains(c.ClinicId)
                        );
                    }

                    if (!string.IsNullOrEmpty(location))
                    {
                        clinicsQuery = clinicsQuery.Where(c => 
                            c.Address != null && c.Address.Contains(location)
                        );
                    }

                    var clinics = await clinicsQuery.ToListAsync();

                    var clinicResults = clinics.Select(c => new SearchResultDto
                    {
                        Id = (int)(c.ClinicId.GetHashCode() % int.MaxValue), // Convert Guid to int
                        Type = "clinic",
                        Name = c.Name ?? "",
                        Specialties = GetClinicSpecialties(c.ClinicId),
                        Address = c.Address ?? "",
                        Image = c.ImageUrl ?? "",
                        Rating = 4.0, // Tạm thời hardcode
                        ReviewCount = 50, // Tạm thời hardcode
                        IsHospital = c.IsHospital,
                        Slug = c.Slug ?? CreateSlug(c.Name ?? ""), // Sử dụng slug từ database trước
                        Highlights = CreateHighlights(c.Name ?? "", c.Address ?? "", q)
                    }).ToList();

                    // Filter by rating
                    if (minRating > 0)
                    {
                        clinicResults = clinicResults.Where(c => c.Rating >= minRating).ToList();
                    }

                    allResults.AddRange(clinicResults);
                }

                // Lấy hospitals từ database (chỉ bệnh viện)
                if (type == "all" || type == "hospital")
                {
                    var hospitalsQuery = _context.Clinics.Where(c => c.IsHospital).AsQueryable();

                    if (!string.IsNullOrEmpty(q))
                    {
                        hospitalsQuery = hospitalsQuery.Where(c => 
                            c.Name.Contains(q) ||
                            (c.Address != null && c.Address.Contains(q))
                        );
                    }

                    if (!string.IsNullOrEmpty(specialty))
                    {
                        var specialtyClinicIds = await _context.DoctorClinicSpecialties
                            .Where(dcs => dcs.Specialty.Name.Contains(specialty))
                            .Select(dcs => dcs.ClinicId)
                            .Distinct()
                            .ToListAsync();

                        hospitalsQuery = hospitalsQuery.Where(c =>
                            specialtyClinicIds.Contains(c.ClinicId)
                        );
                    }

                    if (!string.IsNullOrEmpty(location))
                    {
                        hospitalsQuery = hospitalsQuery.Where(c => 
                            c.Address != null && c.Address.Contains(location)
                        );
                    }

                    var hospitals = await hospitalsQuery.ToListAsync();

                    var hospitalResults = hospitals.Select(c => new SearchResultDto
                    {
                        Id = (int)(c.ClinicId.GetHashCode() % int.MaxValue), // Convert Guid to int
                        Type = "hospital",
                        Name = c.Name ?? "",
                        Specialties = GetClinicSpecialties(c.ClinicId),
                        Address = c.Address ?? "",
                        Image = c.ImageUrl ?? "",
                        Rating = 4.2, // Tạm thời hardcode, cao hơn một chút cho bệnh viện
                        ReviewCount = 150, // Tạm thời hardcode, nhiều hơn cho bệnh viện
                        IsHospital = c.IsHospital,
                        Slug = c.Slug ?? CreateSlug(c.Name ?? ""), // Sử dụng slug từ database trước
                        Highlights = CreateHighlights(c.Name ?? "", c.Address ?? "", q)
                    }).ToList();

                    // Filter by rating
                    if (minRating > 0)
                    {
                        hospitalResults = hospitalResults.Where(c => c.Rating >= minRating).ToList();
                    }

                    allResults.AddRange(hospitalResults);
                }

                // Apply sorting
                allResults = ApplySorting(allResults, sortBy, sortOrder);

                // Pagination
                var totalResults = allResults.Count;
                var pagedResults = allResults
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var response = new SearchResponseDto
                {
                    Results = pagedResults,
                    TotalResults = totalResults,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalResults / pageSize),
                    Query = q,
                    Type = type,
                    Specialty = specialty,
                    Location = location,
                    SortBy = sortBy,
                    SortOrder = sortOrder,
                    Filters = new SearchFiltersDto
                    {
                        MinRating = minRating,
                        MinPrice = minPrice,
                        MaxPrice = maxPrice,
                        OnlineOnly = onlineOnly
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tìm kiếm", error = ex.Message });
            }
        }

        [HttpGet("meilisearch")]
        public async Task<IActionResult> MeiliSearchQuery(
            [FromQuery] string q = "",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12,
            [FromQuery] string type = "all")
        {
            try
            {
                var index = _meiliSearchClient.Index(IndexName);
                
                // Sử dụng SearchAsync đơn giản với query string
                var searchResult = await index.SearchAsync<SearchDocument>(q);

                var allResults = searchResult.Hits.ToList();

                // Filter by type nếu cần
                if (type != "all")
                {
                    allResults = allResults.Where(r => r.Type == type).ToList();
                }

                // Pagination
                var totalResults = allResults.Count;
                var pagedResults = allResults
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var results = pagedResults.Select(hit => new SearchResultDto
                {
                    Id = hit.Id,
                    Type = hit.Type,
                    Name = hit.Name,
                    Title = hit.Title,
                    Specialties = hit.Specialties,
                    Address = hit.Address,
                    Image = hit.Image,
                    Rating = hit.Rating,
                    ReviewCount = hit.ReviewCount,
                    IsOnline = hit.IsOnline,
                    Price = hit.Price,
                    IsHospital = hit.IsHospital,
                    Slug = hit.Slug
                }).ToList();

                var response = new SearchResponseDto
                {
                    Results = results,
                    TotalResults = totalResults,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalResults / pageSize),
                    Query = q,
                    Type = type
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tìm kiếm với MeiliSearch", error = ex.Message });
            }
        }

        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSuggestions([FromQuery] string q)
        {
            try
            {
                var suggestions = new List<SuggestionDto>();

                if (!string.IsNullOrEmpty(q))
                {
                    // Lấy gợi ý từ specialties
                    var specialties = await _context.Specialties
                        .Where(s => s.Name.Contains(q))
                        .Take(3)
                        .Select(s => new SuggestionDto { Text = s.Name, Type = "specialty" })
                        .ToListAsync();
                    
                    suggestions.AddRange(specialties);

                    // Lấy gợi ý từ tên bác sĩ
                    var doctors = await _context.Users
                        .Where(u => u.RoleId == "R2" && u.Name.Contains(q))
                        .Take(3)
                        .Select(u => new SuggestionDto { Text = u.Name, Type = "doctor" })
                        .ToListAsync();
                    
                    suggestions.AddRange(doctors);

                    // Lấy gợi ý từ tên phòng khám
                    var clinics = await _context.Clinics
                        .Where(c => c.Name.Contains(q))
                        .Take(2)
                        .Select(c => new SuggestionDto { Text = c.Name, Type = "clinic" })
                        .ToListAsync();
                    
                    suggestions.AddRange(clinics);
                }

                return Ok(suggestions.Take(5));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy gợi ý", error = ex.Message });
            }
        }

        [HttpPost("index")]
        public async Task<IActionResult> IndexData()
        {
            try
            {
                // Tạo index nếu chưa có
                var index = _meiliSearchClient.Index(IndexName);
                
                var allDocuments = new List<SearchDocument>();

                // Index doctors từ database thông qua DoctorInfo
                var doctorInfos = await _context.DoctorInfos
                    .Include(di => di.Doctor)
                    .Include(di => di.Position)
                    .Include(di => di.Price)
                    .Include(di => di.Clinic)
                    .ToListAsync();

                var doctorDocuments = doctorInfos.Select(di => new SearchDocument
                {
                    Id = di.Id,
                    Type = "doctor",
                    Name = di.Doctor?.Name ?? "",
                    Title = di.PositionId == "P1" ? "Giáo sư" : 
                           di.PositionId == "P2" ? "Phó giáo sư" :
                           di.PositionId == "P3" ? "Tiến sĩ" : 
                           di.PositionId == "P4" ? "Thạc sĩ" : "Bác sĩ",
                    Specialties = GetDoctorSpecialties(di.DoctorId),
                    Address = di.Clinic?.Address ?? "",
                    Image = di.ImageUrl ?? "",
                    Rating = 4.5,
                    ReviewCount = 100,
                    IsOnline = true,
                    Price = 200000, // Tạm thời hardcode
                    Slug = di.Slug ?? CreateSlug(di.Doctor?.Name ?? ""),
                    CreatedAt = DateTime.Now
                });

                allDocuments.AddRange(doctorDocuments);

                // Index clinics và hospitals từ database
                var clinics = await _context.Clinics.ToListAsync();

                var clinicDocuments = clinics.Select(c => new SearchDocument
                {
                    Id = (int)(c.ClinicId.GetHashCode() % int.MaxValue), // Convert Guid to int
                    Type = c.IsHospital ? "hospital" : "clinic", // Phân biệt hospital và clinic
                    Name = c.Name ?? "",
                    Specialties = GetClinicSpecialties(c.ClinicId),
                    Address = c.Address ?? "",
                    Image = c.ImageUrl ?? "",
                    Rating = c.IsHospital ? 4.2 : 4.0, // Bệnh viện có rating cao hơn
                    ReviewCount = c.IsHospital ? 150 : 50, // Bệnh viện có nhiều review hơn
                    IsHospital = c.IsHospital,
                    Slug = c.Slug ?? CreateSlug(c.Name ?? ""), // Sử dụng slug từ database trước
                    CreatedAt = DateTime.Now
                });

                allDocuments.AddRange(clinicDocuments);

                // Thêm documents vào index
                if (allDocuments.Any())
                {
                    await index.AddDocumentsAsync(allDocuments);
                }

                return Ok(new { 
                    message = "Đã index dữ liệu thực từ database vào MeiliSearch",
                    indexName = IndexName,
                    documentsCount = allDocuments.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi index dữ liệu", error = ex.Message });
            }
        }

        [HttpDelete("index")]
        public async Task<IActionResult> ClearIndex()
        {
            try
            {
                var index = _meiliSearchClient.Index(IndexName);
                await index.DeleteAllDocumentsAsync();
                
                return Ok(new { message = "Đã xóa tất cả dữ liệu trong index" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa index", error = ex.Message });
            }
        }

        // Helper method để tạo slug
        private string CreateSlug(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            
            // Chuyển về chữ thường
            text = text.ToLowerInvariant();
            
            // Thay thế các ký tự tiếng Việt
            text = text.Replace("à", "a").Replace("á", "a").Replace("ạ", "a").Replace("ả", "a").Replace("ã", "a")
                      .Replace("â", "a").Replace("ầ", "a").Replace("ấ", "a").Replace("ậ", "a").Replace("ẩ", "a").Replace("ẫ", "a")
                      .Replace("ă", "a").Replace("ằ", "a").Replace("ắ", "a").Replace("ặ", "a").Replace("ẳ", "a").Replace("ẵ", "a")
                      .Replace("è", "e").Replace("é", "e").Replace("ẹ", "e").Replace("ẻ", "e").Replace("ẽ", "e")
                      .Replace("ê", "e").Replace("ề", "e").Replace("ế", "e").Replace("ệ", "e").Replace("ể", "e").Replace("ễ", "e")
                      .Replace("ì", "i").Replace("í", "i").Replace("ị", "i").Replace("ỉ", "i").Replace("ĩ", "i")
                      .Replace("ò", "o").Replace("ó", "o").Replace("ọ", "o").Replace("ỏ", "o").Replace("õ", "o")
                      .Replace("ô", "o").Replace("ồ", "o").Replace("ố", "o").Replace("ộ", "o").Replace("ổ", "o").Replace("ỗ", "o")
                      .Replace("ơ", "o").Replace("ờ", "o").Replace("ớ", "o").Replace("ợ", "o").Replace("ở", "o").Replace("ỡ", "o")
                      .Replace("ù", "u").Replace("ú", "u").Replace("ụ", "u").Replace("ủ", "u").Replace("ũ", "u")
                      .Replace("ư", "u").Replace("ừ", "u").Replace("ứ", "u").Replace("ự", "u").Replace("ử", "u").Replace("ữ", "u")
                      .Replace("ỳ", "y").Replace("ý", "y").Replace("ỵ", "y").Replace("ỷ", "y").Replace("ỹ", "y")
                      .Replace("đ", "d");
            
            // Thay thế khoảng trắng và ký tự đặc biệt bằng dấu gạch ngang
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-z0-9\-]", "-");
            
            // Loại bỏ các dấu gạch ngang liên tiếp
            text = System.Text.RegularExpressions.Regex.Replace(text, @"-+", "-");
            
            // Loại bỏ dấu gạch ngang ở đầu và cuối
            text = text.Trim('-');
            
            return text;
        }

        private List<SearchResultDto> ApplySorting(List<SearchResultDto> results, string sortBy, string sortOrder)
        {
            if (sortBy == "relevance")
            {
                return results.OrderBy(r => r.Name).ToList();
            }
            else if (sortBy == "rating")
            {
                return sortOrder == "desc" ? results.OrderByDescending(r => r.Rating).ToList() : results.OrderBy(r => r.Rating).ToList();
            }
            else if (sortBy == "price")
            {
                return sortOrder == "desc" ? results.OrderByDescending(r => r.Price).ToList() : results.OrderBy(r => r.Price).ToList();
            }
            else if (sortBy == "reviewCount")
            {
                return sortOrder == "desc" ? results.OrderByDescending(r => r.ReviewCount).ToList() : results.OrderBy(r => r.ReviewCount).ToList();
            }
            else if (sortBy == "distance")
            {
                // Implementation of distance sorting
                return results.OrderBy(r => r.Distance).ToList();
            }
            else
            {
                return results;
            }
        }

        private Dictionary<string, IReadOnlyCollection<string>> CreateHighlights(string text, string field, string query)
        {
            var highlights = new Dictionary<string, IReadOnlyCollection<string>>();
            if (!string.IsNullOrEmpty(query))
            {
                var queryTerms = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var term in queryTerms)
                {
                    var matches = new List<string>();
                    var match = text;
                    var index = 0;
                    while ((index = match.IndexOf(term, index, StringComparison.OrdinalIgnoreCase)) != -1)
                    {
                        matches.Add(match.Substring(index, term.Length));
                        index += term.Length;
                    }
                    if (matches.Any())
                    {
                        highlights[field] = matches;
                    }
                }
            }
            return highlights;
        }

        // Helper method để lấy specialties của doctor
        private string[] GetDoctorSpecialties(Guid doctorId)
        {
            try
            {
                return _context.DoctorClinicSpecialties
                    .Where(dcs => dcs.DoctorId == doctorId)
                    .Include(dcs => dcs.Specialty)
                    .Select(dcs => dcs.Specialty.Name)
                    .Distinct()
                    .ToArray();
            }
            catch
            {
                return new string[0];
            }
        }

        // Helper method để lấy specialties của clinic
        private string[] GetClinicSpecialties(Guid clinicId)
        {
            try
            {
                return _context.DoctorClinicSpecialties
                    .Where(dcs => dcs.ClinicId == clinicId)
                    .Include(dcs => dcs.Specialty)
                    .Select(dcs => dcs.Specialty.Name)
                    .Distinct()
                    .ToArray();
            }
            catch
            {
                return new string[0];
            }
        }
    }

    // DTOs
    public class SearchResultDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = "";
        public string Name { get; set; } = "";
        public string Title { get; set; } = "";
        public string[] Specialties { get; set; } = new string[0];
        public string Address { get; set; } = "";
        public string Image { get; set; } = "";
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public bool IsOnline { get; set; }
        public decimal? Price { get; set; }
        public bool IsHospital { get; set; }
        public string Slug { get; set; } = "";
        public double Distance { get; set; } = 0; // Khoảng cách (km)
        public Dictionary<string, IReadOnlyCollection<string>>? Highlights { get; set; }
    }

    public class SearchResponseDto
    {
        public List<SearchResultDto> Results { get; set; } = new();
        public long TotalResults { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public string Query { get; set; } = "";
        public string Type { get; set; } = "";
        public string Specialty { get; set; } = "";
        public string Location { get; set; } = "";
        public string SortBy { get; set; } = "";
        public string SortOrder { get; set; } = "";
        public SearchFiltersDto Filters { get; set; } = new();
    }

    public class SuggestionDto
    {
        public string Text { get; set; } = "";
        public string Type { get; set; } = "";
    }

    public class SearchDocument
    {
        public int Id { get; set; }
        public string Type { get; set; } = "";
        public string Name { get; set; } = "";
        public string Title { get; set; } = "";
        public string[] Specialties { get; set; } = new string[0];
        public string Address { get; set; } = "";
        public string Image { get; set; } = "";
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public bool IsOnline { get; set; }
        public decimal? Price { get; set; }
        public bool IsHospital { get; set; }
        public string Slug { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }

    public class SearchFiltersDto
    {
        public double MinRating { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public bool OnlineOnly { get; set; }
    }
}