using System;
using System.Linq;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.Controllers.Base;
using Hospital_BE.PL.DTOs.Common;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_BE.PL.Controllers
{
    public class ClinicController : BaseController
    {
        private readonly IClinicService _clinicService;

        public ClinicController(IClinicService clinicService)
        {
            _clinicService = clinicService;
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
        /// Tạo mới một phòng khám
        /// </summary>
        /// <param name="model">Thông tin phòng khám cần tạo</param>
        /// <returns>Phòng khám đã được tạo</returns>
        [HttpPost]
        public async Task<IActionResult> CreateClinic([FromBody] Clinic model)
        {
            if (!ModelState.IsValid)
                return ApiBadRequest<object>(ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

            var newClinic = await _clinicService.CreateClinicAsync(model);
            return ApiCreated(newClinic, nameof(GetClinic), new { id = newClinic.ClinicId }, "Tạo phòng khám thành công");
        }

        /// <summary>
        /// Cập nhật thông tin phòng khám
        /// </summary>
        /// <param name="id">ID của phòng khám</param>
        /// <param name="model">Thông tin cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClinic(Guid id, [FromBody] Clinic model)
        {
            if (!ModelState.IsValid)
                return ApiBadRequest<object>(ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

            var result = await _clinicService.UpdateClinicAsync(id, model);

            if (!result)
                return ApiNotFound<object>("Không tìm thấy phòng khám");

            return ApiOk(new { Success = true }, "Cập nhật phòng khám thành công");
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
} 