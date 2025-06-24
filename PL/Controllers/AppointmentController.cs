using System;
using System.Linq;
using System.Threading.Tasks;
using Hospital_BE.BLL.Services;
using Hospital_BE.PL.Controllers.Base;
using Hospital_BE.PL.DTOs.Common;
using Hospital_BE.PL.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_BE.PL.Controllers
{
    public class AppointmentController : BaseController
    {
        private readonly AppointmentService _appointmentService;

        public AppointmentController(AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        /// <summary>
        /// Tạo lịch hẹn mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDTO model)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest<object>(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }

            var result = await _appointmentService.CreateAppointmentAsync(model);
            if (!result.Success)
            {
                return ApiBadRequest<object>(result.Message);
            }

            return ApiCreated(new { appointmentId = result.Data }, nameof(GetAppointment), new { id = result.Data }, "Đặt lịch khám thành công");
        }

        /// <summary>
        /// Lấy thông tin chi tiết lịch hẹn theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointment(Guid id)
        {
            var result = await _appointmentService.GetAppointmentByIdAsync(id);
            if (!result.Success)
            {
                return ApiNotFound<object>(result.Message);
            }

            return ApiOk(result.Data, "Lấy thông tin lịch khám thành công");
        }

        /// <summary>
        /// Lấy danh sách lịch hẹn có phân trang
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAppointments([FromQuery] QueryParameters parameters)
        {
            var result = await _appointmentService.GetAppointmentsAsync(parameters);
            AddPaginationHeader(result);
            return ApiOk(result.Data, "Lấy danh sách lịch khám thành công");
        }

        /// <summary>
        /// Lấy danh sách lịch hẹn theo bệnh nhân
        /// </summary>
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetAppointmentsByPatient(Guid patientId)
        {
            var result = await _appointmentService.GetAppointmentsByPatientIdAsync(patientId);
            if (!result.Success)
            {
                return ApiBadRequest<object>(result.Message);
            }

            return ApiOk(result.Data, "Lấy danh sách lịch khám theo bệnh nhân thành công");
        }

        /// <summary>
        /// Lấy danh sách lịch hẹn theo bác sĩ
        /// </summary>
        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetAppointmentsByDoctor(Guid doctorId)
        {
            var result = await _appointmentService.GetAppointmentsByDoctorIdAsync(doctorId);
            if (!result.Success)
            {
                return ApiBadRequest<object>(result.Message);
            }

            return ApiOk(result.Data, "Lấy danh sách lịch khám theo bác sĩ thành công");
        }

        /// <summary>
        /// Lấy danh sách lịch hẹn theo người dùng
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAppointmentsByUser(Guid userId)
        {
            var result = await _appointmentService.GetAppointmentsByUserIdAsync(userId);
            if (!result.Success)
            {
                return ApiBadRequest<object>(result.Message);
            }

            return ApiOk(result.Data, "Lấy danh sách lịch khám theo người dùng thành công");
        }

        /// <summary>
        /// Cập nhật trạng thái lịch hẹn
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateAppointmentStatus(Guid id, [FromBody] UpdateStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest<object>(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }

            var result = await _appointmentService.UpdateAppointmentStatusAsync(id, request.Status);
            if (!result.Success)
            {
                if (result.Message.Contains("không tìm thấy"))
                {
                    return ApiNotFound<object>(result.Message);
                }
                return ApiBadRequest<object>(result.Message);
            }

            return ApiOk(new { success = true }, result.Message);
        }

        /// <summary>
        /// Hủy lịch hẹn
        /// </summary>
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(Guid id)
        {
            var result = await _appointmentService.CancelAppointmentAsync(id);
            if (!result.Success)
            {
                if (result.Message.Contains("không tìm thấy"))
                {
                    return ApiNotFound<object>(result.Message);
                }
                return ApiBadRequest<object>(result.Message);
            }

            return ApiOk(new { success = true }, "Hủy lịch khám thành công");
        }

        /// <summary>
        /// Xác nhận lịch hẹn
        /// </summary>
        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> ConfirmAppointment(Guid id)
        {
            var result = await _appointmentService.ConfirmAppointmentAsync(id);
            if (!result.Success)
            {
                if (result.Message.Contains("không tìm thấy"))
                {
                    return ApiNotFound<object>(result.Message);
                }
                return ApiBadRequest<object>(result.Message);
            }

            return ApiOk(new { success = true }, result.Message);
        }

        /// <summary>
        /// Xác nhận lịch hẹn qua email (không cần authentication)
        /// </summary>
        [HttpGet("confirm/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmAppointmentByEmail(Guid id, [FromQuery] string token = "")
        {
            // Có thể thêm validation token nếu cần bảo mật hơn
            var result = await _appointmentService.ConfirmAppointmentAsync(id);
            if (!result.Success)
            {
                if (result.Message.Contains("không tìm thấy"))
                {
                    return ApiNotFound<object>(result.Message);
                }
                return ApiBadRequest<object>(result.Message);
            }

            // Trả về một trang HTML đơn giản thông báo thành công
            var htmlContent = @"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Xác nhận lịch khám</title>
                    <meta charset='utf-8'>
                    <style>
                        body { font-family: Arial, sans-serif; text-align: center; padding: 50px; }
                        .success { color: #28a745; }
                        .container { max-width: 600px; margin: 0 auto; }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h2 class='success'>✅ Xác nhận lịch khám thành công!</h2>
                        <p>Lịch khám của bạn đã được xác nhận. Vui lòng đến đúng giờ hẹn.</p>
                        <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi.</p>
                    </div>
                </body>
                </html>";

            return Content(htmlContent, "text/html");
        }
    }

    // DTO cho request update status
    public class UpdateStatusRequest
    {
        public string Status { get; set; }
    }
} 