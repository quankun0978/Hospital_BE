using Hospital_BE.DAL.Context;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace Hospital_BE.DAL.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly ApplicationDbContext _context;

        public ScheduleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<Schedule>> GetSchedulesAsync(QueryParameters parameters)
        {
            var query = _context.Schedules
                .Include(s => s.Doctor)
                .Include(s => s.TimeTypeAllcode)
                .AsQueryable();

            if (!string.IsNullOrEmpty(parameters.SearchTerm))
            {
                query = query.Where(s => s.Doctor.Name.Contains(parameters.SearchTerm) ||
                                       s.TimeTypeAllcode.ValueVi.Contains(parameters.SearchTerm));
            }

            query = query.OrderByDescending(s => s.Date).ThenBy(s => s.TimeType);

            var totalCount = await query.CountAsync();
            var schedules = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PaginatedResult<Schedule>(
                schedules,
                totalCount,
                parameters.PageNumber,
                parameters.PageSize
            );
        }

        public async Task<PaginatedResult<Schedule>> GetSchedulesByDoctorAsync(Guid doctorId, QueryParameters parameters)
        {
            var query = _context.Schedules
                .Include(s => s.Doctor)
                .Include(s => s.TimeTypeAllcode)
                .Where(s => s.DoctorId == doctorId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(parameters.SearchTerm))
            {
                query = query.Where(s => s.TimeTypeAllcode.ValueVi.Contains(parameters.SearchTerm));
            }

            query = query.OrderByDescending(s => s.Date).ThenBy(s => s.TimeType);

            var totalCount = await query.CountAsync();
            var schedules = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PaginatedResult<Schedule>(
                schedules,
                totalCount,
                parameters.PageNumber,
                parameters.PageSize
            );
        }

        public async Task<Schedule?> GetScheduleByIdAsync(int id)
        {
            return await _context.Schedules
                .Include(s => s.Doctor)
                .Include(s => s.TimeTypeAllcode)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Schedule> CreateScheduleAsync(Schedule schedule)
        {
            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task<Schedule> UpdateScheduleAsync(Schedule schedule)
        {
            _context.Schedules.Update(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task<bool> DeleteScheduleAsync(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null) return false;

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckScheduleExistsAsync(Guid doctorId, DateTime date, string timeType)
        {
            return await _context.Schedules
                .AnyAsync(s => s.DoctorId == doctorId && 
                              s.Date.Date == date.Date && 
                              s.TimeType == timeType);
        }

        public async Task<Schedule?> GetScheduleByDoctorAndDateTimeAsync(Guid doctorId, DateTime date, string timeType)
        {
            return await _context.Schedules
                .FirstOrDefaultAsync(s => s.DoctorId == doctorId && 
                                         s.Date.Date == date.Date && 
                                         s.TimeType == timeType);
        }

        public async Task<IEnumerable<Schedule>> GetSchedulesByDateRangeAsync(Guid doctorId, DateTime startDate, DateTime endDate)
        {
            return await _context.Schedules
                .Include(s => s.Doctor)
                .Include(s => s.TimeTypeAllcode)
                .Where(s => s.DoctorId == doctorId && s.Date >= startDate && s.Date <= endDate)
                .OrderBy(s => s.Date)
                .ThenBy(s => s.TimeTypeAllcode.ValueVi)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy danh sách lịch khám của bác sĩ theo doctorId
        /// </summary>
        /// <param name="doctorId">ID của bác sĩ</param>
        /// <returns>Danh sách lịch khám</returns>
        public async Task<IEnumerable<Schedule>> GetSchedulesByDoctorIdAsync(Guid doctorId)
        {
            return await _context.Schedules
                .Include(s => s.Doctor)
                .Include(s => s.TimeTypeAllcode)
                .Where(s => s.DoctorId == doctorId && s.Date >= DateTime.Today)
                .OrderBy(s => s.Date)
                .ThenBy(s => s.TimeTypeAllcode.ValueVi)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy danh sách lịch khám của bác sĩ theo slug
        /// </summary>
        /// <param name="slug">Slug của bác sĩ</param>
        /// <returns>Danh sách lịch khám</returns>
        public async Task<IEnumerable<Schedule>> GetSchedulesByDoctorSlugAsync(string slug)
        {
            return await _context.Schedules
                .Include(s => s.Doctor)
                    .ThenInclude(d => d.DoctorInfos)
                .Include(s => s.TimeTypeAllcode)
                .Where(s => s.Doctor.DoctorInfos.Any(di => di.Slug == slug) && s.Date >= DateTime.Today)
                .OrderBy(s => s.Date)
                .ThenBy(s => s.TimeTypeAllcode.ValueVi)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy lịch khám theo khoảng thời gian (overload)
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu</param>
        /// <param name="endDate">Ngày kết thúc</param>
        /// <param name="doctorId">ID bác sĩ (tùy chọn)</param>
        /// <returns>Danh sách lịch khám</returns>
        public async Task<IEnumerable<Schedule>> GetSchedulesByDateRangeAsync(DateTime startDate, DateTime endDate, Guid? doctorId = null)
        {
            var query = _context.Schedules
                .Include(s => s.Doctor)
                .Include(s => s.TimeTypeAllcode)
                .Where(s => s.Date >= startDate && s.Date <= endDate);

            if (doctorId.HasValue)
            {
                query = query.Where(s => s.DoctorId == doctorId.Value);
            }

            return await query
                .OrderBy(s => s.Date)
                .ThenBy(s => s.TimeTypeAllcode.ValueVi)
                .ToListAsync();
        }
    }
} 