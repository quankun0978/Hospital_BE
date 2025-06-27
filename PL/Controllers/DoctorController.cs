using System;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.DAL.Models;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.PL.Controllers.Base;
using Hospital_BE.PL.DTOs.Common;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Collections.Generic;

namespace Hospital_BE.PL.Controllers
{
    public class DoctorController : BaseController
    {
        private readonly IDoctorService _doctorService;
        private readonly IMarkdownService _markdownService;
        private readonly IImageService _imageService;
        private readonly IDoctorClinicSpecialtyRepository _doctorClinicSpecialtyRepository;

        public DoctorController(
            IDoctorService doctorService, 
            IMarkdownService markdownService, 
            IImageService imageService,
            IDoctorClinicSpecialtyRepository doctorClinicSpecialtyRepository)
        {
            _doctorService = doctorService;
            _markdownService = markdownService;
            _imageService = imageService;
            _doctorClinicSpecialtyRepository = doctorClinicSpecialtyRepository;
        }

        /// <summary>
        /// Lấy danh sách bác sĩ có phân trang
        /// </summary>
        /// <param name="parameters">Tham số phân trang</param>
        /// <returns>Danh sách bác sĩ đã phân trang</returns>
        [HttpGet]
        public async Task<IActionResult> GetDoctors([FromQuery] QueryParameters parameters)
        {
            var result = await _doctorService.GetDoctorsAsync(parameters);
            AddPaginationHeader(result);
            return ApiOk(result.Data, "Lấy danh sách bác sĩ thành công");
        }

        /// <summary>
        /// Lấy thông tin chi tiết bác sĩ theo ID
        /// </summary>
        /// <param name="id">ID của bác sĩ</param>
        /// <returns>Thông tin chi tiết bác sĩ</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctor(Guid id)
        {
            var doctorInfo = await _doctorService.GetDoctorByIdAsync(id);

            if (doctorInfo == null)
                return ApiNotFound<object>("Không tìm thấy thông tin bác sĩ");

            return ApiOk(doctorInfo, "Lấy chi tiết bác sĩ thành công");
        }

        /// <summary>
        /// Lấy thông tin chi tiết bác sĩ theo slug
        /// </summary>
        /// <param name="slug">Slug của bác sĩ</param>
        /// <returns>Thông tin chi tiết bác sĩ</returns>
        [HttpGet("by-slug/{slug}")]
        public async Task<IActionResult> GetDoctorBySlug(string slug)
        {
            var doctorInfo = await _doctorService.GetDoctorBySlugAsync(slug);

            if (doctorInfo == null)
                return ApiNotFound<object>("Không tìm thấy thông tin bác sĩ");

            return ApiOk(doctorInfo, "Lấy chi tiết bác sĩ thành công");
        }

        /// <summary>
        /// Tạo mới thông tin bác sĩ
        /// </summary>
        /// <param name="request">Thông tin bác sĩ cần tạo</param>
        /// <returns>Thông tin bác sĩ đã được tạo</returns>
        [HttpPost]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ApiBadRequest<object>(ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

                // Create doctor info
                var doctorInfo = new DoctorInfo
                {
                    DoctorId = request.DoctorId,
                    PriceId = request.PriceId,
                    PositionId = request.PositionId,
                    ClinicId = request.ClinicId,
                    Slug = request.Slug,
                    Note = request.Note,
                    ImageUrl = request.ImageUrl,
                    Count = request.Count ?? 0
                };

                var newDoctor = await _doctorService.CreateDoctorInfoAsync(doctorInfo);

                // Create markdown content if provided
                if (!string.IsNullOrEmpty(request.ContentHtml) || !string.IsNullOrEmpty(request.ContentMarkdown))
                {
                    var markdown = new Markdown
                    {
                        DoctorId = newDoctor.DoctorId,
                        ContentHTML = request.ContentHtml ?? "",
                        ContentMarkdown = request.ContentMarkdown ?? "",
                        Description = request.Note
                    };

                    await _markdownService.CreateMarkdownAsync(markdown);
                }

                // Create specialty relationships if provided
                if (request.SelectedSpecialties != null && request.SelectedSpecialties.Any())
                {
                    foreach (var specialtyId in request.SelectedSpecialties)
                    {
                        var doctorClinicSpecialty = new DoctorClinicSpecialty
                        {
                            DoctorId = newDoctor.DoctorId,
                            ClinicId = request.ClinicId ?? Guid.Empty, // Use default if not provided
                            SpecialtyId = specialtyId
                        };

                        await _doctorClinicSpecialtyRepository.CreateAsync(doctorClinicSpecialty);
                    }
                }

                return ApiCreated(newDoctor, nameof(GetDoctor), new { id = newDoctor.DoctorId }, "Tạo thông tin bác sĩ thành công");
            }
            catch (Exception ex)
            {
                return ApiBadRequest<object>(ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật thông tin bác sĩ
        /// </summary>
        /// <param name="id">ID của bác sĩ</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(Guid id, [FromBody] UpdateDoctorDto request)
        {
            try
            {
                // Get existing doctor
                var existingDoctor = await _doctorService.GetDoctorByIdAsync(id);
                if (existingDoctor == null || existingDoctor.DoctorInfos == null || !existingDoctor.DoctorInfos.Any())
                    return ApiNotFound<object>("Không tìm thấy thông tin bác sĩ");

                var existingInfo = existingDoctor.DoctorInfos.First();

                // Update doctor info
                var doctorInfo = new DoctorInfo
                {
                    DoctorId = id,
                    PriceId = request.PriceId ?? existingInfo.PriceId,
                    PositionId = request.PositionId ?? existingInfo.PositionId,
                    ClinicId = request.ClinicId ?? existingInfo.ClinicId,
                    Slug = request.Slug ?? existingInfo.Slug,
                    Note = request.Note ?? existingInfo.Note,
                    ImageUrl = request.ImageUrl ?? existingInfo.ImageUrl,
                    Count = request.Count ?? existingInfo.Count
                };

                var result = await _doctorService.UpdateDoctorInfoAsync(id, doctorInfo);

                if (!result)
                    return ApiNotFound<object>("Không tìm thấy thông tin bác sĩ");

                // Update markdown content
                var existingMarkdown = await _markdownService.GetMarkdownByDoctorIdAsync(id);

                if (!string.IsNullOrEmpty(request.ContentHtml) || !string.IsNullOrEmpty(request.ContentMarkdown))
                {
                    if (existingMarkdown != null)
                    {
                        // Update existing markdown
                        existingMarkdown.ContentHTML = request.ContentHtml ?? existingMarkdown.ContentHTML;
                        existingMarkdown.ContentMarkdown = request.ContentMarkdown ?? existingMarkdown.ContentMarkdown;
                        existingMarkdown.Description = request.Note ?? existingMarkdown.Description;

                        await _markdownService.UpdateMarkdownAsync(existingMarkdown);
                    }
                    else
                    {
                        // Create new markdown
                        var markdown = new Markdown
                        {
                            DoctorId = id,
                            ContentHTML = request.ContentHtml ?? "",
                            ContentMarkdown = request.ContentMarkdown ?? "",
                            Description = request.Note
                        };

                        await _markdownService.CreateMarkdownAsync(markdown);
                    }
                }

                // Update specialty relationships
                if (request.SelectedSpecialties != null)
                {
                    // Delete existing relationships
                    await _doctorClinicSpecialtyRepository.DeleteByDoctorIdAsync(id);

                    // Create new relationships
                    if (request.SelectedSpecialties.Any())
                    {
                        foreach (var specialtyId in request.SelectedSpecialties)
                        {
                            var doctorClinicSpecialty = new DoctorClinicSpecialty
                            {
                                DoctorId = id,
                                ClinicId = request.ClinicId ?? existingInfo.ClinicId ?? Guid.Empty,
                                SpecialtyId = specialtyId
                            };

                            await _doctorClinicSpecialtyRepository.CreateAsync(doctorClinicSpecialty);
                        }
                    }
                }

                return ApiOk(new { Success = true }, "Cập nhật thông tin bác sĩ thành công");
            }
            catch (Exception ex)
            {
                return ApiBadRequest<object>(ex.Message);
            }
        }

        /// <summary>
        /// Lấy nội dung markdown của bác sĩ
        /// </summary>
        /// <param name="id">ID của bác sĩ</param>
        /// <returns>Nội dung markdown</returns>
        [HttpGet("{id}/markdown")]
        public async Task<IActionResult> GetDoctorMarkdown(Guid id)
        {
            var markdown = await _markdownService.GetMarkdownByDoctorIdAsync(id);

            if (markdown == null)
                return ApiNotFound<object>("Không tìm thấy nội dung markdown");

            var result = new
            {
                Id = markdown.Id,
                DoctorId = markdown.DoctorId,
                ContentHTML = markdown.ContentHTML,
                ContentMarkdown = markdown.ContentMarkdown,
                Description = markdown.Description
            };

            return ApiOk(result, "Lấy nội dung markdown thành công");
        }

        /// <summary>
        /// Xóa thông tin bác sĩ
        /// </summary>
        /// <param name="id">ID của bác sĩ</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(Guid id)
        {
            // Delete associated markdown content first
            var existingMarkdown = await _markdownService.GetMarkdownByDoctorIdAsync(id);
            if (existingMarkdown != null)
            {
                await _markdownService.DeleteMarkdownAsync(existingMarkdown.Id);
            }

            var result = await _doctorService.DeleteDoctorInfoAsync(id);

            if (!result)
                return ApiNotFound<object>("Không tìm thấy thông tin bác sĩ");

            return ApiNoContent();
        }
    }

    /// <summary>
    /// DTO for creating doctor with JSON data
    /// </summary>
    public class CreateDoctorDto
    {
        [Required(ErrorMessage = "ID bác sĩ là bắt buộc")]
        public Guid DoctorId { get; set; }

        [Required(ErrorMessage = "Mức giá là bắt buộc")]
        public string PriceId { get; set; }

        public string PositionId { get; set; }
        public Guid? ClinicId { get; set; }
        public string Slug { get; set; }
        public string Note { get; set; }
        public string ImageUrl { get; set; }
        public int? Count { get; set; }
        public string ContentHtml { get; set; }
        public string ContentMarkdown { get; set; }
        public List<Guid> SelectedSpecialties { get; set; } = new List<Guid>();
    }

    /// <summary>
    /// DTO for updating doctor with JSON data
    /// </summary>
    public class UpdateDoctorDto
    {
        public string PriceId { get; set; }
        public string PositionId { get; set; }
        public Guid? ClinicId { get; set; }
        public string Slug { get; set; }
        public string Note { get; set; }
        public string ImageUrl { get; set; }
        public int? Count { get; set; }
        public string ContentHtml { get; set; }
        public string ContentMarkdown { get; set; }
        public List<Guid> SelectedSpecialties { get; set; } = new List<Guid>();
    }
} 