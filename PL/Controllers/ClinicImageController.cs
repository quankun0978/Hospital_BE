using System;
using System.Linq;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_BE.PL.Controllers
{
    public class ClinicImageController : BaseController
    {
        private readonly IClinicImageService _clinicImageService;

        public ClinicImageController(IClinicImageService clinicImageService)
        {
            _clinicImageService = clinicImageService;
        }

        /// <summary>
        /// Lấy tất cả ảnh của tất cả các phòng khám
        /// </summary>
        /// <returns>Danh sách tất cả ảnh</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllImages()
        {
            var images = await _clinicImageService.GetAllImagesAsync();
            return ApiOk(images, "Lấy danh sách ảnh thành công");
        }

        /// <summary>
        /// Lấy ảnh theo ID
        /// </summary>
        /// <param name="id">ID của ảnh</param>
        /// <returns>Thông tin ảnh</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            var image = await _clinicImageService.GetImageByIdAsync(id);

            if (image == null)
                return ApiNotFound<object>("Không tìm thấy ảnh");

            return ApiOk(image, "Lấy thông tin ảnh thành công");
        }

        /// <summary>
        /// Lấy tất cả ảnh của một phòng khám
        /// </summary>
        /// <param name="clinicId">ID của phòng khám</param>
        /// <returns>Danh sách ảnh của phòng khám</returns>
        [HttpGet("clinic/{clinicId}")]
        public async Task<IActionResult> GetImagesByClinicId(Guid clinicId)
        {
            var images = await _clinicImageService.GetImagesByClinicIdAsync(clinicId);
            return ApiOk(images, "Lấy danh sách ảnh của phòng khám thành công");
        }

        /// <summary>
        /// Lấy ảnh background của một phòng khám
        /// </summary>
        /// <param name="clinicId">ID của phòng khám</param>
        /// <returns>Danh sách ảnh background</returns>
        [HttpGet("clinic/{clinicId}/background")]
        public async Task<IActionResult> GetBackgroundImages(Guid clinicId)
        {
            var images = await _clinicImageService.GetBackgroundImagesByClinicIdAsync(clinicId);
            return ApiOk(images, "Lấy danh sách ảnh background thành công");
        }

        /// <summary>
        /// Lấy ảnh fallback của một phòng khám
        /// </summary>
        /// <param name="clinicId">ID của phòng khám</param>
        /// <returns>Danh sách ảnh fallback</returns>
        [HttpGet("clinic/{clinicId}/fallback")]
        public async Task<IActionResult> GetFallbackImages(Guid clinicId)
        {
            var images = await _clinicImageService.GetFallbackImagesByClinicIdAsync(clinicId);
            return ApiOk(images, "Lấy danh sách ảnh fallback thành công");
        }

        /// <summary>
        /// Tạo mới một ảnh cho phòng khám
        /// </summary>
        /// <param name="model">Thông tin ảnh cần tạo</param>
        /// <returns>Ảnh đã được tạo</returns>
        [HttpPost]
        public async Task<IActionResult> CreateImage([FromBody] ClinicImage model)
        {
            if (!ModelState.IsValid)
                return ApiBadRequest<object>(ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

            try
            {
                var newImage = await _clinicImageService.CreateImageAsync(model);
                return ApiCreated(newImage, nameof(GetImage), new { id = newImage.Id }, "Tạo ảnh thành công");
            }
            catch (ArgumentException ex)
            {
                return ApiBadRequest<object>(ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật thông tin ảnh
        /// </summary>
        /// <param name="id">ID của ảnh</param>
        /// <param name="model">Thông tin cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateImage(int id, [FromBody] ClinicImage model)
        {
            if (!ModelState.IsValid)
                return ApiBadRequest<object>(ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

            try
            {
                var result = await _clinicImageService.UpdateImageAsync(id, model);

                if (!result)
                    return ApiNotFound<object>("Không tìm thấy ảnh");

                return ApiOk(new { Success = true }, "Cập nhật ảnh thành công");
            }
            catch (ArgumentException ex)
            {
                return ApiBadRequest<object>(ex.Message);
            }
        }

        /// <summary>
        /// Xóa một ảnh
        /// </summary>
        /// <param name="id">ID của ảnh</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var result = await _clinicImageService.DeleteImageAsync(id);

            if (!result)
                return ApiNotFound<object>("Không tìm thấy ảnh");

            return ApiNoContent();
        }

        /// <summary>
        /// Xóa tất cả ảnh của một phòng khám
        /// </summary>
        /// <param name="clinicId">ID của phòng khám</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("clinic/{clinicId}")]
        public async Task<IActionResult> DeleteImagesByClinicId(Guid clinicId)
        {
            var result = await _clinicImageService.DeleteImagesByClinicIdAsync(clinicId);
            return ApiOk(new { Success = result }, "Xóa ảnh của phòng khám thành công");
        }
    }
} 