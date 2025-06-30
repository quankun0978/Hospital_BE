using Hospital_BE.BLL.Interfaces;
using Hospital_BE.BLL.Models;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs;
using Hospital_BE.PL.DTOs.Common;
using Hospital_BE.DAL.Context;
using Microsoft.EntityFrameworkCore;

namespace Hospital_BE.BLL.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ApplicationDbContext _context;

        public ScheduleService(IScheduleRepository scheduleRepository, ApplicationDbContext context)
        {
            _scheduleRepository = scheduleRepository;
            _context = context;
        }

        public async Task<ServiceResult<PaginatedResult<Schedule>>> GetSchedulesAsync(QueryParameters parameters)
        {
            try
            {
                var result = await _scheduleRepository.GetSchedulesAsync(parameters);
                return ServiceResult<PaginatedResult<Schedule>>.Ok("Lấy danh sách lịch khám thành công", result);
            }
            catch (Exception ex)
            {
                return ServiceResult<PaginatedResult<Schedule>>.Error($"Lỗi khi lấy danh sách lịch khám: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PaginatedResult<Schedule>>> GetSchedulesByDoctorAsync(Guid doctorId, QueryParameters parameters)
        {
            try
            {
                var result = await _scheduleRepository.GetSchedulesByDoctorAsync(doctorId, parameters);
                return ServiceResult<PaginatedResult<Schedule>>.Ok("Lấy lịch khám của bác sĩ thành công", result);
            }
            catch (Exception ex)
            {
                return ServiceResult<PaginatedResult<Schedule>>.Error($"Lỗi khi lấy lịch khám của bác sĩ: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Schedule>> GetScheduleByIdAsync(int id)
        {
            try
            {
                var schedule = await _scheduleRepository.GetScheduleByIdAsync(id);
                if (schedule == null)
                {
                    return ServiceResult<Schedule>.Error("Không tìm thấy lịch khám");
                }
                return ServiceResult<Schedule>.Ok("Lấy thông tin lịch khám thành công", schedule);
            }
            catch (Exception ex)
            {
                return ServiceResult<Schedule>.Error($"Lỗi khi lấy thông tin lịch khám: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Schedule>> CreateScheduleAsync(Schedule schedule)
        {
            try
            {
                // Kiểm tra trùng lịch
                var existingSchedule = await _scheduleRepository.GetScheduleByDoctorAndDateTimeAsync(
                    schedule.DoctorId, schedule.Date, schedule.TimeType);
                
                if (existingSchedule != null)
                {
                    return ServiceResult<Schedule>.Error("Bác sĩ đã có lịch khám vào thời gian này");
                }

                var result = await _scheduleRepository.CreateScheduleAsync(schedule);
                return ServiceResult<Schedule>.Ok("Tạo lịch khám thành công", result);
            }
            catch (Exception ex)
            {
                return ServiceResult<Schedule>.Error($"Lỗi khi tạo lịch khám: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Schedule>> UpdateScheduleAsync(int id, Schedule schedule)
        {
            try
            {
                var existingSchedule = await _scheduleRepository.GetScheduleByIdAsync(id);
                if (existingSchedule == null)
                {
                    return ServiceResult<Schedule>.Error("Không tìm thấy lịch khám");
                }

                // Kiểm tra trùng lịch (ngoại trừ chính nó)
                var duplicateSchedule = await _scheduleRepository.GetScheduleByDoctorAndDateTimeAsync(
                    schedule.DoctorId, schedule.Date, schedule.TimeType);
                
                if (duplicateSchedule != null && duplicateSchedule.Id != id)
                {
                    return ServiceResult<Schedule>.Error("Bác sĩ đã có lịch khám vào thời gian này");
                }

                existingSchedule.Date = schedule.Date;
                existingSchedule.TimeType = schedule.TimeType;
                existingSchedule.DoctorId = schedule.DoctorId;
                existingSchedule.IsActive = schedule.IsActive;

                var result = await _scheduleRepository.UpdateScheduleAsync(existingSchedule);
                return ServiceResult<Schedule>.Ok("Cập nhật lịch khám thành công", result);
            }
            catch (Exception ex)
            {
                return ServiceResult<Schedule>.Error($"Lỗi khi cập nhật lịch khám: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteScheduleAsync(int id)
        {
            try
            {
                var schedule = await _scheduleRepository.GetScheduleByIdAsync(id);
                if (schedule == null)
                {
                    return ServiceResult<bool>.Error("Không tìm thấy lịch khám");
                }
                
                var result = await _scheduleRepository.DeleteScheduleAsync(id);
                return ServiceResult<bool>.Ok("Xóa lịch khám thành công", result);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Error($"Lỗi khi xóa lịch khám: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<Schedule>>> GetSchedulesByDateRangeAsync(DateTime startDate, DateTime endDate, Guid? doctorId = null)
        {
            try
            {
                var schedules = await _scheduleRepository.GetSchedulesByDateRangeAsync(startDate, endDate, doctorId);
                return ServiceResult<List<Schedule>>.Ok("Lấy lịch khám theo khoảng thời gian thành công", schedules.ToList());
            }
            catch (Exception ex)
            {
                return ServiceResult<List<Schedule>>.Error($"Lỗi khi lấy lịch khám theo khoảng thời gian: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy danh sách lịch khám của bác sĩ theo doctorId
        /// </summary>
        /// <param name="doctorId">ID của bác sĩ</param>
        /// <returns>Danh sách lịch khám</returns>
        public async Task<IEnumerable<ScheduleResponseDto>> GetSchedulesByDoctorIdAsync(Guid doctorId)
        {
            try
            {
                var schedules = await _scheduleRepository.GetSchedulesByDoctorIdAsync(doctorId);
                var scheduleList = schedules.ToList();
                
                // Kiểm tra appointments đã đặt cho từng schedule
                var result = new List<ScheduleResponseDto>();
                
                foreach (var schedule in scheduleList)
                {
                    // Kiểm tra xem slot này đã có appointment chưa (loại trừ trạng thái hủy S4)
                    var hasAppointment = await _context.Appointments
                        .AnyAsync(a => a.DoctorId == schedule.DoctorId && 
                                      a.AppointmentDate.Date == schedule.Date.Date && 
                                      a.TimeType == schedule.TimeType &&
                                      a.Status != "S4"); // Loại trừ lịch đã hủy
                    
                    result.Add(new ScheduleResponseDto
                {
                        Id = schedule.Id,
                        Date = schedule.Date,
                        TimeType = schedule.TimeType,
                        DoctorId = schedule.DoctorId,
                        DoctorName = schedule.Doctor?.Name,
                        TimeTypeText = schedule.TimeTypeAllcode?.ValueVi,
                        IsActive = schedule.IsActive && !hasAppointment // Disable nếu schedule inactive hoặc đã có appointment
                });
                }
                
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy lịch khám của bác sĩ: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy danh sách lịch khám của bác sĩ theo slug
        /// </summary>
        /// <param name="slug">Slug của bác sĩ</param>
        /// <param name="date">Ngày cần lấy lịch (optional)</param>
        /// <returns>Danh sách lịch khám</returns>
        public async Task<IEnumerable<ScheduleResponseDto>> GetSchedulesByDoctorSlugAsync(string slug, DateTime? date = null)
        {
            try
            {
                var schedules = await _scheduleRepository.GetSchedulesByDoctorSlugAsync(slug);
                
                // Nếu có date, filter theo ngày đó
                if (date.HasValue)
                {
                    schedules = schedules.Where(s => s.Date.Date == date.Value.Date);
                }
                
                var scheduleList = schedules.ToList();
                
                // Kiểm tra appointments đã đặt cho từng schedule
                var result = new List<ScheduleResponseDto>();
                
                foreach (var schedule in scheduleList)
                {
                    // Kiểm tra xem slot này đã có appointment chưa (loại trừ trạng thái hủy S4)
                    var hasAppointment = await _context.Appointments
                        .AnyAsync(a => a.DoctorId == schedule.DoctorId && 
                                      a.AppointmentDate.Date == schedule.Date.Date && 
                                      a.TimeType == schedule.TimeType &&
                                      a.Status != "S4"); // Loại trừ lịch đã hủy
                    
                    result.Add(new ScheduleResponseDto
                {
                        Id = schedule.Id,
                        Date = schedule.Date,
                        TimeType = schedule.TimeType,
                        DoctorId = schedule.DoctorId,
                        DoctorName = schedule.Doctor?.Name,
                        TimeTypeText = schedule.TimeTypeAllcode?.ValueVi,
                        IsActive = schedule.IsActive && !hasAppointment // Disable nếu schedule inactive hoặc đã có appointment
                });
                }
                
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy lịch khám của bác sĩ: {ex.Message}");
            }
        }
    }
} 