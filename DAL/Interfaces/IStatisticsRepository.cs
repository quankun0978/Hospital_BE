using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital_BE.PL.DTOs;

namespace Hospital_BE.DAL.Interfaces
{
    public interface IStatisticsRepository
    {
        /// <summary>
        /// Lấy thống kê doanh thu theo khoảng thời gian
        /// </summary>
        Task<RevenueStatisticsDto> GetRevenueStatisticsAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Lấy doanh thu theo ngày
        /// </summary>
        Task<List<DailyRevenueDto>> GetDailyRevenueAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Lấy doanh thu theo bác sĩ
        /// </summary>
        Task<List<DoctorRevenueDto>> GetDoctorRevenueAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Lấy dữ liệu cho biểu đồ doanh thu
        /// </summary>
        Task<RevenueChartDto> GetRevenueChartDataAsync(DateTime fromDate, DateTime toDate);
    }
} 