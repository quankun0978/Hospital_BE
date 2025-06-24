using System;
using System.Linq;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.Controllers.Base;
using Hospital_BE.PL.DTOs.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel;

namespace Hospital_BE.PL.Controllers
{
    public class ClinicController : BaseController
    {
        private readonly IClinicService _clinicService;
        private readonly IImageService _imageService;

        public ClinicController(IClinicService clinicService, IImageService imageService)
        {
            _clinicService = clinicService;
            _imageService = imageService;
        }

        /// <summary>
        /// Lấy danh sách phòng khám có phân trang
        /// </summary>
        /// <param name="parameters">Tham số phân trang</param>
        /// <returns>Danh sách phòng khám đã phân trang</returns>
        [HttpGet]
        public async Task<IActionResult> GetClinics([FromQuery] QueryParameters parameters)
        {
            var result = await _clinicService.GetClinicsAsync(parameters);
            AddPaginationHeader(result);
            return ApiOk(result.Data, "Lấy danh sách phòng khám thành công");
        }

        /// <summary>
        /// Lấy thông tin chi tiết phòng khám theo ID
        /// </summary>
        /// <param name="id">ID của phòng khám</param>
        /// <returns>Thông tin chi tiết phòng khám</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClinic(Guid id)
        {
            var clinic = await _clinicService.GetClinicByIdAsync(id);

            if (clinic == null)
                return ApiNotFound<object>("Không tìm thấy phòng khám");

            return ApiOk(clinic, "Lấy chi tiết phòng khám thành công");
        }

        /// <summary>
        /// Lấy thông tin chi tiết phòng khám theo slug
        /// </summary>
        /// <param name="slug">Slug của phòng khám</param>
        /// <returns>Thông tin chi tiết phòng khám</returns>
        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetClinicBySlug(string slug)
        {
            var clinic = await _clinicService.GetClinicBySlugAsync(slug);

            if (clinic == null)
                return ApiNotFound<object>("Không tìm thấy phòng khám");

            return ApiOk(clinic, "Lấy chi tiết phòng khám thành công");
        }

        /// <summary>
        /// Tạo mới một phòng khám
        /// </summary>
        /// <param name="request">Thông tin phòng khám cần tạo</param>
        /// <returns>Phòng khám đã được tạo</returns>
        [HttpPost]
        public async Task<IActionResult> CreateClinic([FromBody] CreateClinicDto request)
        {
            try
            {
                if (!ModelState.IsValid) 
                    return ApiBadRequest<object>(ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

                // Create clinic
                var clinic = new Clinic
                {
                    Name = request.Name,
                    Address = request.Address,
                    Description = request.Description,
                    Slug = request.Slug,
                    ImageUrl = request.ImageUrl,
                    LogoImg = request.LogoImg,
                    IsHospital = request.IsHospital ?? false
                };

                var newClinic = await _clinicService.CreateClinicAsync(clinic);

                return ApiCreated(newClinic, nameof(GetClinic), new { id = newClinic.ClinicId }, "Tạo phòng khám thành công");
            }
            catch (Exception ex)
            {
                return ApiBadRequest<object>(ex.Message);
            }
        }

        /// <summary>
        /// Tạo mới một phòng khám với upload file
        /// </summary>
        /// <param name="request">Thông tin phòng khám cần tạo với files</param>
        /// <returns>Phòng khám đã được tạo</returns>
        [HttpPost("with-files")]
        public async Task<IActionResult> CreateClinicWithFiles([FromForm] CreateClinicRequest request)
        {
            try
            {
                if (!ModelState.IsValid) 
                    return ApiBadRequest<object>(ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

                // Handle image upload
                string imageUrl = null;
                if (request.ImageFile != null)
                {
                    imageUrl = await _imageService.UploadImageAsync(request.ImageFile, "clinics");
                }
                else if (!string.IsNullOrEmpty(request.ImageUrl))
                {
                    imageUrl = request.ImageUrl;
                }

                // Handle logo upload
                string logoUrl = null;
                if (request.LogoFile != null)
                {
                    logoUrl = await _imageService.UploadImageAsync(request.LogoFile, "logos");
                }
                else if (!string.IsNullOrEmpty(request.LogoImg))
                {
                    logoUrl = request.LogoImg;
                }

                // Create clinic
                var clinic = new Clinic
                {
                    Name = request.Name,
                    Address = request.Address,
                    Description = request.Description,
                    Slug = request.Slug,
                    ImageUrl = imageUrl,
                    LogoImg = logoUrl,
                    IsHospital = request.IsHospital ?? false
                };

                var newClinic = await _clinicService.CreateClinicAsync(clinic);

                // Insert additional images into ClinicImage table if provided
                if (request.AdditionalImages != null && request.AdditionalImages.Any())
                {
                    foreach (var additionalImage in request.AdditionalImages)
                    {
                        if (additionalImage != null)
                        {
                            var additionalImageUrl = await _imageService.UploadImageAsync(additionalImage, "clinics");
                            await _clinicService.AddClinicImageAsync(newClinic.ClinicId, additionalImageUrl, false);
                        }
                    }
                }

                // Insert background image if provided
                if (request.BackgroundImageFile != null)
                {
                    var backgroundImageUrl = await _imageService.UploadImageAsync(request.BackgroundImageFile, "clinics");
                    await _clinicService.AddClinicImageAsync(newClinic.ClinicId, backgroundImageUrl, true);
                }

                return ApiCreated(newClinic, nameof(GetClinic), new { id = newClinic.ClinicId }, "Tạo phòng khám thành công");
            }
            catch (Exception ex)
            {
                return ApiBadRequest<object>(ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật thông tin phòng khám
        /// </summary>
        /// <param name="id">ID của phòng khám</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClinic(Guid id, [FromBody] UpdateClinicDto request)
        {
            try
            {
                // Get existing clinic
                var existingClinic = await _clinicService.GetClinicByIdAsync(id);
                if (existingClinic == null)
                    return ApiNotFound<object>("Không tìm thấy phòng khám");

                // Update clinic
                var clinic = new Clinic
                {
                    Name = request.Name ?? existingClinic.Name,
                    Address = request.Address ?? existingClinic.Address,
                    Description = request.Description ?? existingClinic.Description,
                    Slug = request.Slug ?? existingClinic.Slug,
                    ImageUrl = request.ImageUrl ?? existingClinic.ImageUrl,
                    LogoImg = request.LogoImg ?? existingClinic.LogoImg,
                    IsHospital = request.IsHospital ?? existingClinic.IsHospital
                };

                var result = await _clinicService.UpdateClinicAsync(id, clinic);

                if (!result)
                    return ApiNotFound<object>("Không tìm thấy phòng khám");

                return ApiOk(new { Success = true }, "Cập nhật phòng khám thành công");
            }
            catch (Exception ex)
            {
                return ApiBadRequest<object>(ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật thông tin phòng khám với upload file
        /// </summary>
        /// <param name="id">ID của phòng khám</param>
        /// <param name="request">Thông tin cập nhật với files</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("{id}/with-files")]
        public async Task<IActionResult> UpdateClinicWithFiles(Guid id, [FromForm] UpdateClinicRequest request)
        {
            try
            {
                // Get existing clinic
                var existingClinic = await _clinicService.GetClinicByIdAsync(id);
                if (existingClinic == null)
                    return ApiNotFound<object>("Không tìm thấy phòng khám");

                // Handle image upload
                if (request.ImageFile != null)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(existingClinic.ImageUrl))
                    {
                        await _imageService.DeleteImageAsync(existingClinic.ImageUrl);
                    }

                    // Upload new image
                    request.ImageUrl = await _imageService.UploadImageAsync(request.ImageFile, "clinics");
                }

                // Handle logo upload
                if (request.LogoFile != null)
                {
                    // Delete old logo if exists
                    if (!string.IsNullOrEmpty(existingClinic.LogoImg))
                    {
                        await _imageService.DeleteImageAsync(existingClinic.LogoImg);
                    }

                    // Upload new logo
                    request.LogoImg = await _imageService.UploadImageAsync(request.LogoFile, "logos");
                }

                // Update clinic
                var clinic = new Clinic
                {
                    Name = request.Name ?? existingClinic.Name,
                    Address = request.Address ?? existingClinic.Address,
                    Description = request.Description ?? existingClinic.Description,
                    Slug = request.Slug ?? existingClinic.Slug,
                    ImageUrl = request.ImageUrl ?? existingClinic.ImageUrl,
                    LogoImg = request.LogoImg ?? existingClinic.LogoImg,
                    IsHospital = request.IsHospital ?? existingClinic.IsHospital
                };

                var result = await _clinicService.UpdateClinicAsync(id, clinic);

                if (!result)
                    return ApiNotFound<object>("Không tìm thấy phòng khám");

                return ApiOk(new { Success = true }, "Cập nhật phòng khám thành công");
            }
            catch (Exception ex)
            {
                return ApiBadRequest<object>(ex.Message);
            }
        }

        /// <summary>
        /// Upload ảnh cho phòng khám
        /// </summary>
        /// <param name="file">File ảnh</param>
        /// <param name="folder">Thư mục lưu (mặc định: clinics)</param>
        /// <returns>URL của ảnh đã upload</returns>
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] string folder = "clinics")
        {
            try
            {
                if (file == null || file.Length == 0)
                    return ApiBadRequest<object>("Không có file được chọn");

                if (!_imageService.IsValidImageFile(file))
                    return ApiBadRequest<object>("Định dạng file không hợp lệ");

                var imageUrl = await _imageService.UploadImageAsync(file, folder);
                return ApiOk(new { imageUrl }, "Upload ảnh thành công");
            }
            catch (Exception ex)
            {
                return ApiBadRequest<object>(ex.Message);
            }
        }

        /// <summary>
        /// Xóa một phòng khám
        /// </summary>
        /// <param name="id">ID của phòng khám</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClinic(Guid id)
        {
            var result = await _clinicService.DeleteClinicAsync(id);

            if (!result)
                return ApiNotFound<object>("Không tìm thấy phòng khám");

            return ApiNoContent();
        }
    }

    /// <summary>
    /// Request model for updating clinic with file uploads
    /// </summary>
    public class UpdateClinicRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public string ImageUrl { get; set; }
        public string LogoImg { get; set; }
        public bool? IsHospital { get; set; }
        public IFormFile ImageFile { get; set; }
        public IFormFile LogoFile { get; set; }
    }

    /// <summary>
    /// DTO for updating clinic with JSON data
    /// </summary>
    public class UpdateClinicDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public string ImageUrl { get; set; }
        public string LogoImg { get; set; }
        public bool? IsHospital { get; set; }
    }

    /// <summary>
    /// DTO for creating clinic with JSON data
    /// </summary>
    public class CreateClinicDto
    {
        [Required(ErrorMessage = "Tên cơ sở y tế là bắt buộc")]
        public string Name { get; set; }
        
        public string Address { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public string ImageUrl { get; set; }
        public string LogoImg { get; set; }
        public bool? IsHospital { get; set; }
    }

    /// <summary>
    /// Request model for creating clinic with file uploads
    /// </summary>
    public class CreateClinicRequest
    {
        [Required(ErrorMessage = "Tên cơ sở y tế là bắt buộc")]
        public string Name { get; set; }
        
        public string Address { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public string ImageUrl { get; set; }
        public string LogoImg { get; set; }
        public bool? IsHospital { get; set; }
        
        // File uploads - không bắt buộc
        [DefaultValue(null)]
        public IFormFile ImageFile { get; set; }
        
        [DefaultValue(null)]
        public IFormFile LogoFile { get; set; }
        
        [DefaultValue(null)]
        public IFormFile BackgroundImageFile { get; set; }
        
        [DefaultValue(null)]
        public List<IFormFile> AdditionalImages { get; set; }
    }
} 