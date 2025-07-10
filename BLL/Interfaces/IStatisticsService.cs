using System;
using System.Threading.Tasks;
using Hospital_BE.BLL.Models;
using Hospital_BE.PL.DTOs;

namespace Hospital_BE.BLL.Interfaces
{
    public interface IStatisticsService
    {
        /// <summary>
        /// Lấy thống kê doanh thu theo khoảng thời gian
        /// </summary>
        Task<ServiceResult<RevenueStatisticsDto>> GetRevenueStatisticsAsync(RevenueStatisticsRequestDto request);

        /// <summary>
        /// Lấy dữ liệu cho biểu đồ doanh thu
        /// </summary>
        Task<ServiceResult<RevenueChartDto>> GetRevenueChartDataAsync(RevenueStatisticsRequestDto request);

        /// <summary>
        /// Xuất báo cáo doanh thu ra file Excel
        /// </summary>
        Task<ServiceResult<byte[]>> ExportRevenueReportAsync(RevenueStatisticsRequestDto request);
    }
} 