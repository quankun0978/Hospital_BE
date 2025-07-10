using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.Controllers.Base;
using Hospital_BE.PL.DTOs;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ScheduleController : BaseController
    {
        private readonly IScheduleService _scheduleService;
        private readonly IUserService _userService;

        public ScheduleController(IScheduleService scheduleService, IUserService userService)
        {
            _scheduleService = scheduleService;
            _userService = userService;
        }

        /// <summary>
        /// Lấy danh sách lịch khám (chỉ Admin R1)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "R1")]
        public async Task<IActionResult> GetSchedules([FromQuery] QueryParameters parameters)
        {
            var result = await _scheduleService.GetSchedulesAsync(parameters);
            if (!result.Success)
            {
                return ApiBadRequest(result.Message);
            }

            AddPaginationHeader(result.Data);
            
            var responseData = result.Data.Data.Select(s => new ScheduleResponseDto
            {
                Id = s.Id,
                Date = s.Date,
                TimeType = s.TimeType,
                TimeTypeText = s.TimeTypeAllcode?.ValueVi ?? s.TimeType,
                DoctorId = s.DoctorId,
                DoctorName = s.Doctor?.Name ?? "",
                DoctorEmail = s.Doctor?.Email ?? "",
                IsActive = s.IsActive
            }).ToList();

            var response = new PaginatedResult<ScheduleResponseDto>(
                responseData,
                result.Data.TotalCount,
                result.Data.CurrentPage,
                result.Data.PageSize
            );

            return ApiOk(response, "Lấy danh sách lịch khám thành công");
        }

        /// <summary>
        /// Lấy lịch khám của bác sĩ hiện tại (R2) hoặc theo doctorId (R1)
        /// </summary>
        [HttpGet("doctor")]
        public async Task<IActionResult> GetSchedulesByDoctor([FromQuery] QueryParameters parameters, [FromQuery] Guid? doctorId = null)
        {
            var currentUserId = GetCurrentUserIdAsGuid();
            var currentUserRole = GetCurrentUserRole();

            Guid targetDoctorId;
            
            // Admin có thể xem lịch của bất kỳ bác sĩ nào
            if (currentUserRole == "R1" && doctorId.HasValue)
            {
                targetDoctorId = doctorId.Value;
                
                // Kiểm tra bác sĩ có tồn tại không
                var doctorExists = await _userService.CheckUserExistsAsync(targetDoctorId, "R2");
                if (!doctorExists.Success || !doctorExists.Data)
                {
                    return ApiBadRequest("Bác sĩ không tồn tại");
                }
            }
            // Bác sĩ chỉ có thể xem lịch của chính mình
            else
            {
                targetDoctorId = currentUserId;
            }

            var result = await _scheduleService.GetSchedulesByDoctorAsync(targetDoctorId, parameters);
            if (!result.Success)
            {
                return ApiBadRequest(result.Message);
            }

            AddPaginationHeader(result.Data);
            
            var responseData = result.Data.Data.Select(s => new ScheduleResponseDto
            {
                Id = s.Id,
                Date = s.Date,
                TimeType = s.TimeType,
                TimeTypeText = s.TimeTypeAllcode?.ValueVi ?? s.TimeType,
                DoctorId = s.DoctorId,
                DoctorName = s.Doctor?.Name ?? "",
                DoctorEmail = s.Doctor?.Email ?? "",
                IsActive = s.IsActive
            }).ToList();

            var response = new PaginatedResult<ScheduleResponseDto>(
                responseData,
                result.Data.TotalCount,
                result.Data.CurrentPage,
                result.Data.PageSize
            );

            return ApiOk(response, "Lấy lịch khám thành công");
        }

        /// <summary>
        /// Lấy chi tiết lịch khám
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetScheduleById(int id)
        {
            var result = await _scheduleService.GetScheduleByIdAsync(id);
            if (!result.Success)
            {
                return ApiBadRequest(result.Message);
            }

            var currentUserId = GetCurrentUserIdAsGuid();
            var currentUserRole = GetCurrentUserRole();

            // Kiểm tra quyền truy cập
            if (currentUserRole == "R2" && result.Data.DoctorId != currentUserId)
            {
                return ApiForbidden("Bạn không có quyền xem lịch khám này");
            }

            var response = new ScheduleResponseDto
            {
                Id = result.Data.Id,
                Date = result.Data.Date,
                TimeType = result.Data.TimeType,
                TimeTypeText = result.Data.TimeTypeAllcode?.ValueVi ?? result.Data.TimeType,
                DoctorId = result.Data.DoctorId,
                DoctorName = result.Data.Doctor?.Name ?? "",
                DoctorEmail = result.Data.Doctor?.Email ?? "",
                IsActive = result.Data.IsActive
            };

            return ApiOk(response, "Lấy thông tin lịch khám thành công");
        }

        /// <summary>
        /// Tạo lịch khám mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleDto dto)
        {
            var currentUserId = GetCurrentUserIdAsGuid();
            var currentUserRole = GetCurrentUserRole();

            // Bác sĩ chỉ có thể tạo lịch cho chính mình
            if (currentUserRole == "R2")
            {
                dto.DoctorId = currentUserId;
            }
            // Admin có thể tạo lịch cho bất kỳ bác sĩ nào
            else if (currentUserRole == "R1")
            {
                // Kiểm tra bác sĩ có tồn tại không
                var doctorExists = await _userService.CheckUserExistsAsync(dto.DoctorId, "R2");
                if (!doctorExists.Success || !doctorExists.Data)
                {
                    return ApiBadRequest("Bác sĩ không tồn tại");
                }
            }

            var schedule = new Schedule
            {
                Date = dto.Date,
                TimeType = dto.TimeType,
                DoctorId = dto.DoctorId,
                IsActive = dto.IsActive
            };

            var result = await _scheduleService.CreateScheduleAsync(schedule);
            if (!result.Success)
            {
                return ApiBadRequest(result.Message);
            }

            return ApiOk(result.Data, result.Message);
        }

        /// <summary>
        /// Cập nhật lịch khám
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] UpdateScheduleDto dto)
        {
            var currentUserId = GetCurrentUserIdAsGuid();
            var currentUserRole = GetCurrentUserRole();

            // Kiểm tra lịch khám tồn tại và quyền truy cập
            var existingSchedule = await _scheduleService.GetScheduleByIdAsync(id);
            if (!existingSchedule.Success)
            {
                return ApiBadRequest(existingSchedule.Message);
            }

            // Bác sĩ chỉ có thể sửa lịch của chính mình
            if (currentUserRole == "R2")
            {
                if (existingSchedule.Data.DoctorId != currentUserId)
                {
                    return ApiForbidden("Bạn không có quyền sửa lịch khám này");
                }
                dto.DoctorId = currentUserId;
            }
            // Admin có thể sửa lịch của bất kỳ bác sĩ nào
            else if (currentUserRole == "R1")
            {
                // Kiểm tra bác sĩ có tồn tại không
                var doctorExists = await _userService.CheckUserExistsAsync(dto.DoctorId, "R2");
                if (!doctorExists.Success || !doctorExists.Data)
                {
                    return ApiBadRequest("Bác sĩ không tồn tại");
                }
            }

            var schedule = new Schedule
            {
                Date = dto.Date,
                TimeType = dto.TimeType,
                DoctorId = dto.DoctorId,
                IsActive = dto.IsActive
            };

            var result = await _scheduleService.UpdateScheduleAsync(id, schedule);
            if (!result.Success)
            {
                return ApiBadRequest(result.Message);
            }

            return ApiOk(result.Data, result.Message);
        }

        /// <summary>
        /// Xóa lịch khám
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var currentUserId = GetCurrentUserIdAsGuid();
            var currentUserRole = GetCurrentUserRole();

            // Kiểm tra lịch khám tồn tại và quyền truy cập
            var existingSchedule = await _scheduleService.GetScheduleByIdAsync(id);
            if (!existingSchedule.Success)
            {
                return ApiBadRequest(existingSchedule.Message);
            }

            // Bác sĩ chỉ có thể xóa lịch của chính mình
            if (currentUserRole == "R2" && existingSchedule.Data.DoctorId != currentUserId)
            {
                return ApiForbidden("Bạn không có quyền xóa lịch khám này");
            }

            var result = await _scheduleService.DeleteScheduleAsync(id);
            if (!result.Success)
            {
                return ApiBadRequest(result.Message);
            }

            return ApiOk(result.Data, result.Message);
        }

        /// <summary>
        /// Lấy danh sách bác sĩ (chỉ Admin R1)
        /// </summary>
        [HttpGet("doctors")]
        [Authorize(Roles = "R1")]
        public async Task<IActionResult> GetDoctors()
        {
            var result = await _userService.GetUsersByRoleAsync("R2");
            if (!result.Success)
            {
                return ApiBadRequest(result.Message);
            }

            var doctors = result.Data.Select(u => new DoctorOptionDto
            {
                Id = u.UserId,
                Name = u.Name,
                Email = u.Email
            }).ToList();

            return ApiOk(doctors, "Lấy danh sách bác sĩ thành công");
        }

        /// <summary>
        /// Lấy lịch khám theo khoảng thời gian
        /// </summary>
        [HttpGet("date-range")]
        public async Task<IActionResult> GetSchedulesByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] Guid? doctorId = null)
        {
            var currentUserId = GetCurrentUserIdAsGuid();
            var currentUserRole = GetCurrentUserRole();

            Guid targetDoctorId;
            
            // Admin có thể xem lịch của bất kỳ bác sĩ nào
            if (currentUserRole == "R1" && doctorId.HasValue)
            {
                targetDoctorId = doctorId.Value;
            }
            // Bác sĩ chỉ có thể xem lịch của chính mình
            else
            {
                targetDoctorId = currentUserId;
            }

            var result = await _scheduleService.GetSchedulesByDateRangeAsync(startDate, endDate, targetDoctorId);
            if (!result.Success)
            {
                return ApiBadRequest(result.Message);
            }

            var responseData = result.Data.Select(s => new ScheduleResponseDto
            {
                Id = s.Id,
                Date = s.Date,
                TimeType = s.TimeType,
                TimeTypeText = s.TimeTypeAllcode?.ValueVi ?? s.TimeType,
                DoctorId = s.DoctorId,
                DoctorName = s.Doctor?.Name ?? "",
                DoctorEmail = s.Doctor?.Email ?? "",
                IsActive = s.IsActive
            }).ToList();

            return ApiOk(responseData, "Lấy lịch khám theo khoảng thời gian thành công");
        }

        /// <summary>
        /// Lấy danh sách lịch khám của bác sĩ theo doctorId
        /// </summary>
        /// <param name="doctorId">ID của bác sĩ</param>
        /// <returns>Danh sách lịch khám</returns>
        [HttpGet("doctor/{doctorId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSchedulesByDoctorId(Guid doctorId)
        {
            try
            {
                var schedules = await _scheduleService.GetSchedulesByDoctorIdAsync(doctorId);
                return Ok(new ApiResponse<IEnumerable<ScheduleResponseDto>>
                {
                    Succeeded = true,
                    Data = schedules,
                    Message = "Lấy danh sách lịch khám thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<IEnumerable<ScheduleResponseDto>>
                {
                    Succeeded = false,
                    Message = $"Lỗi khi lấy danh sách lịch khám: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Lấy danh sách lịch khám của bác sĩ theo slug
        /// </summary>
        /// <param name="slug">Slug của bác sĩ</param>
        /// <param name="date">Ngày cần lấy lịch (optional, mặc định là hôm nay)</param>
        /// <returns>Danh sách lịch khám</returns>
        [HttpGet("doctor/slug/{slug}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSchedulesByDoctorSlug(string slug, [FromQuery] DateTime? date = null)
        {
            try
            {
                // Nếu không truyền date, mặc định lấy từ hôm nay
                var targetDate = date ?? DateTime.Today;
                
                var schedules = await _scheduleService.GetSchedulesByDoctorSlugAsync(slug, targetDate);
                return Ok(new ApiResponse<IEnumerable<ScheduleResponseDto>>
                {
                    Succeeded = true,
                    Data = schedules,
                    Message = "Lấy danh sách lịch khám thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<IEnumerable<ScheduleResponseDto>>
                {
                    Succeeded = false,
                    Message = $"Lỗi khi lấy danh sách lịch khám: {ex.Message}"
                });
            }
        }

        // Methods GetCurrentUserId và GetCurrentUserRole đã được kế thừa từ BaseController
    }
} 