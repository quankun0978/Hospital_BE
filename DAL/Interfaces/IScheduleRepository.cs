using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.DAL.Interfaces
{
    public interface IScheduleRepository
    {
        Task<PaginatedResult<Schedule>> GetSchedulesAsync(QueryParameters parameters);
        Task<PaginatedResult<Schedule>> GetSchedulesByDoctorAsync(Guid doctorId, QueryParameters parameters);
        Task<Schedule?> GetScheduleByIdAsync(int id);
        Task<Schedule> CreateScheduleAsync(Schedule schedule);
        Task<Schedule> UpdateScheduleAsync(Schedule schedule);
        Task<bool> DeleteScheduleAsync(int id);
        Task<bool> CheckScheduleExistsAsync(Guid doctorId, DateTime date, string timeType);
        Task<Schedule?> GetScheduleByDoctorAndDateTimeAsync(Guid doctorId, DateTime date, string timeType);
        /// <summary>
        /// Lấy lịch khám theo khoảng thời gian
        /// </summary>
        /// <param name="doctorId">ID bác sĩ</param>
        /// <param name="startDate">Ngày bắt đầu</param>
        /// <param name="endDate">Ngày kết thúc</param>
        /// <returns>Danh sách lịch khám</returns>
        Task<IEnumerable<Schedule>> GetSchedulesByDateRangeAsync(Guid doctorId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Lấy danh sách lịch khám của bác sĩ theo doctorId
        /// </summary>
        /// <param name="doctorId">ID của bác sĩ</param>
        /// <returns>Danh sách lịch khám</returns>
        Task<IEnumerable<Schedule>> GetSchedulesByDoctorIdAsync(Guid doctorId);

        /// <summary>
        /// Lấy danh sách lịch khám của bác sĩ theo slug
        /// </summary>
        /// <param name="slug">Slug của bác sĩ</param>
        /// <returns>Danh sách lịch khám</returns>
        Task<IEnumerable<Schedule>> GetSchedulesByDoctorSlugAsync(string slug);

        /// <summary>
        /// Lấy lịch khám theo khoảng thời gian (overload)
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu</param>
        /// <param name="endDate">Ngày kết thúc</param>
        /// <param name="doctorId">ID bác sĩ (tùy chọn)</param>
        /// <returns>Danh sách lịch khám</returns>
        Task<IEnumerable<Schedule>> GetSchedulesByDateRangeAsync(DateTime startDate, DateTime endDate, Guid? doctorId = null);
    }
} 