using System;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.PL.Controllers.Base;
using Hospital_BE.PL.DTOs;
using Hospital_BE.PL.DTOs.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Hospital_BE.PL.Controllers
{
    /// <summary>
    /// Controller quản lý chuyên khoa
    /// </summary>
    public class SpecialtyController : BaseController
    {
        private readonly ISpecialtyService _specialtyService;
        private readonly IImageService _imageService;

        public SpecialtyController(ISpecialtyService specialtyService, IImageService imageService)
        {
            _specialtyService = specialtyService;
            _imageService = imageService;
        }

        /// <summary>
        /// Lấy danh sách tất cả chuyên khoa
        /// </summary>
        /// <param name="parameters">Tham số phân trang</param>
        /// <returns>Danh sách chuyên khoa</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllSpecialties([FromQuery] PaginationParameters parameters)
        {
            try
            {
                var result = await _specialtyService.GetAllSpecialtiesAsync(parameters);
                AddPaginationHeader(result);
                return ApiOk(result.Data, "Lấy danh sách chuyên khoa thành công");
            }
            catch (Exception ex)
            {
                return ApiBadRequest<object>($"Lỗi khi lấy danh sách chuyên khoa: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy thông tin chuyên khoa theo ID
        /// </summary>
        /// <param name="id">ID chuyên khoa</param>
        /// <returns>Thông tin chuyên khoa</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSpecialtyById(Guid id)
        {
            try
            {
                var specialty = await _specialtyService.GetSpecialtyByIdAsync(id);
                if (specialty == null)
                    return ApiNotFound<object>("Không tìm thấy chuyên khoa");

                return ApiOk(specialty, "Lấy thông tin chuyên khoa thành công");
            }
            catch (Exception ex)
            {
                return ApiBadRequest<object>($"Lỗi khi lấy thông tin chuyên khoa: {ex.Message}");
            }
        }

        /// <summary>
        /// Tạo mới chuyên khoa
        /// </summary>
        /// <param name="createDto">Thông tin chuyên khoa mới</param>
        /// <returns>Thông tin chuyên khoa đã tạo</returns>
        [HttpPost]
        public async Task<IActionResult> CreateSpecialty([FromBody] CreateSpecialtyDTO createDto)
        {
            if (!ModelState.IsValid)
                return ApiBadRequest<object>("Dữ liệu không hợp lệ");

            try
            {
                var createdSpecialty = await _specialtyService.CreateSpecialtyAsync(createDto);
                return ApiCreated(createdSpecialty, nameof(GetSpecialtyById), new { id = createdSpecialty.SpecialtyId }, "Tạo chuyên khoa thành công");
            }
            catch (InvalidOperationException ex)
            {
                return ApiBadRequest<object>(ex.Message);
            }
            catch (Exception ex)
            {
                return ApiBadRequest<object>($"Lỗi khi tạo chuyên khoa: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật thông tin chuyên khoa
        /// </summary>
        /// <param name="id">ID chuyên khoa</param>
        /// <param name="updateDto">Thông tin cần cập nhật</param>
        /// <returns>Thông tin chuyên khoa đã cập nhật</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpecialty(Guid id, [FromBody] UpdateSpecialtyDTO updateDto)
        {
            if (!ModelState.IsValid)
                return ApiBadRequest<object>("Dữ liệu không hợp lệ");

            try
            {
                var updatedSpecialty = await _specialtyService.UpdateSpecialtyAsync(id, updateDto);
                if (updatedSpecialty == null)
                    return ApiNotFound<object>("Không tìm thấy chuyên khoa để cập nhật");

                return ApiOk(updatedSpecialty, "Cập nhật chuyên khoa thành công");
            }
            catch (InvalidOperationException ex)
            {
                return ApiBadRequest<object>(ex.Message);
            }
            catch (Exception ex)
            {
                return ApiBadRequest<object>($"Lỗi khi cập nhật chuyên khoa: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa chuyên khoa
        /// </summary>
        /// <param name="id">ID chuyên khoa</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpecialty(Guid id)
        {
            try
            {
                var result = await _specialtyService.DeleteSpecialtyAsync(id);
                if (!result)
                    return ApiNotFound<object>("Không tìm thấy chuyên khoa để xóa");

                return ApiNoContent();
            }
            catch (Exception ex)
            {
                return ApiBadRequest<object>($"Lỗi khi xóa chuyên khoa: {ex.Message}");
            }
        }

        /// <summary>
        /// Tạo slug từ tên chuyên khoa
        /// </summary>
        /// <param name="request">Tên chuyên khoa</param>
        /// <returns>Slug được tạo</returns>
        [HttpPost("generate-slug")]
        public IActionResult GenerateSlug([FromBody] SpecialtySlugRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
                return ApiBadRequest<object>("Tên chuyên khoa không được để trống");

            try
            {
                var slug = GenerateSlugFromName(request.Name);
                return ApiOk(new { slug }, "Tạo slug thành công");
            }
            catch (Exception ex)
            {
                return ApiBadRequest<object>($"Lỗi khi tạo slug: {ex.Message}");
            }
        }

        private string GenerateSlugFromName(string name)
        {
            return name?.ToLower()
                .Replace(" ", "-")
                .Replace("ă", "a")
                .Replace("â", "a")
                .Replace("đ", "d")
                .Replace("ê", "e")
                .Replace("ô", "o")
                .Replace("ơ", "o")
                .Replace("ư", "u")
                .Replace("à", "a")
                .Replace("á", "a")
                .Replace("ạ", "a")
                .Replace("ả", "a")
                .Replace("ã", "a")
                .Replace("è", "e")
                .Replace("é", "e")
                .Replace("ẹ", "e")
                .Replace("ẻ", "e")
                .Replace("ẽ", "e")
                .Replace("ì", "i")
                .Replace("í", "i")
                .Replace("ị", "i")
                .Replace("ỉ", "i")
                .Replace("ĩ", "i")
                .Replace("ò", "o")
                .Replace("ó", "o")
                .Replace("ọ", "o")
                .Replace("ỏ", "o")
                .Replace("õ", "o")
                .Replace("ù", "u")
                .Replace("ú", "u")
                .Replace("ụ", "u")
                .Replace("ủ", "u")
                .Replace("ũ", "u")
                .Replace("ỳ", "y")
                .Replace("ý", "y")
                .Replace("ỵ", "y")
                .Replace("ỷ", "y")
                .Replace("ỹ", "y");
        }

        /// <summary>
        /// Upload hình ảnh cho chuyên khoa
        /// </summary>
        /// <param name="file">File hình ảnh</param>
        /// <returns>URL của hình ảnh đã upload</returns>
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadSpecialtyImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return ApiBadRequest<object>("Vui lòng chọn file hình ảnh");

                var imageUrl = await _imageService.UploadImageAsync(file, "speciality");
                return ApiOk(new { imageUrl }, "Tải ảnh lên thành công");
            }
            catch (Exception ex)
            {
                return ApiBadRequest<object>($"Lỗi khi tải ảnh lên: {ex.Message}");
            }
        }
    }

    public class SpecialtySlugRequest
    {
        public string Name { get; set; }
    }
} 