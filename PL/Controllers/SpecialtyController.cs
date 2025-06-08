using System;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.PL.Controllers.Base;
using Hospital_BE.PL.DTOs;
using Hospital_BE.PL.DTOs.Common;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_BE.PL.Controllers
{
    /// <summary>
    /// Controller quản lý chuyên khoa
    /// </summary>
    public class SpecialtyController : BaseController
    {
        private readonly ISpecialtyService _specialtyService;

        public SpecialtyController(ISpecialtyService specialtyService)
        {
            _specialtyService = specialtyService;
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
    }
} 