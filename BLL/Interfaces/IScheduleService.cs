using Hospital_BE.BLL.Models;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.BLL.Interfaces
{
    public interface IScheduleService
    {
        Task<ServiceResult<PaginatedResult<Schedule>>> GetSchedulesAsync(QueryParameters parameters);
        Task<ServiceResult<PaginatedResult<Schedule>>> GetSchedulesByDoctorAsync(Guid doctorId, QueryParameters parameters);
        Task<ServiceResult<Schedule>> GetScheduleByIdAsync(int id);
        Task<ServiceResult<Schedule>> CreateScheduleAsync(Schedule schedule);
        Task<ServiceResult<Schedule>> UpdateScheduleAsync(int id, Schedule schedule);
        Task<ServiceResult<bool>> DeleteScheduleAsync(int id);
        
        /// <summary>
        /// Lấy lịch khám theo khoảng thời gian
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu</param>
        /// <param name="endDate">Ngày kết thúc</param>
        /// <param name="doctorId">ID bác sĩ (tùy chọn)</param>
        /// <returns>Danh sách lịch khám</returns>
        Task<ServiceResult<List<Schedule>>> GetSchedulesByDateRangeAsync(DateTime startDate, DateTime endDate, Guid? doctorId = null);

        /// <summary>
        /// Lấy danh sách lịch khám của bác sĩ theo doctorId
        /// </summary>
        /// <param name="doctorId">ID của bác sĩ</param>
        /// <returns>Danh sách lịch khám</returns>
        Task<IEnumerable<ScheduleResponseDto>> GetSchedulesByDoctorIdAsync(Guid doctorId);

        /// <summary>
        /// Lấy danh sách lịch khám của bác sĩ theo slug
        /// </summary>
        /// <param name="slug">Slug của bác sĩ</param>
        /// <param name="date">Ngày cần lấy lịch (optional)</param>
        /// <returns>Danh sách lịch khám</returns>
        Task<IEnumerable<ScheduleResponseDto>> GetSchedulesByDoctorSlugAsync(string slug, DateTime? date = null);
    }
} 