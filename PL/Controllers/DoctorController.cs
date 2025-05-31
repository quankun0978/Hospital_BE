using System;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.PL.Controllers.Base;
using Hospital_BE.PL.DTOs.Common;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_BE.PL.Controllers
{
    public class DoctorController : BaseController
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
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
            return Ok(result);
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
                return ApiBadRequest<object>("Không tìm thấy thông tin bác sĩ");

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
                return ApiBadRequest<object>("Không tìm thấy thông tin bác sĩ");

            return ApiOk(doctorInfo, "Lấy chi tiết bác sĩ thành công");
        }
    }
} 