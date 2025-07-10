using System;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.BLL.Models;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.PL.DTOs;
using Microsoft.Extensions.Logging;

namespace Hospital_BE.BLL.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly ILogger<StatisticsService> _logger;

        public StatisticsService(
            IStatisticsRepository statisticsRepository,
            ILogger<StatisticsService> logger)
        {
            _statisticsRepository = statisticsRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<RevenueStatisticsDto>> GetRevenueStatisticsAsync(RevenueStatisticsRequestDto request)
        {
            try
            {
                // Validate input
                if (request.FromDate > request.ToDate)
                {
                    return ServiceResult<RevenueStatisticsDto>.Error("Ngày bắt đầu không thể lớn hơn ngày kết thúc");
                }

                if (request.ToDate > DateTime.Now.Date)
                {
                    return ServiceResult<RevenueStatisticsDto>.Error("Ngày kết thúc không thể lớn hơn ngày hiện tại");
                }

                // Giới hạn khoảng thời gian không quá 1 năm
                var daysDiff = (request.ToDate - request.FromDate).Days;
                if (daysDiff > 365)
                {
                    return ServiceResult<RevenueStatisticsDto>.Error("Khoảng thời gian thống kê không được vượt quá 365 ngày");
                }

                var statistics = await _statisticsRepository.GetRevenueStatisticsAsync(request.FromDate, request.ToDate);
                
                return ServiceResult<RevenueStatisticsDto>.Ok("Lấy thống kê doanh thu thành công", statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting revenue statistics for period {FromDate} to {ToDate}", 
                    request.FromDate, request.ToDate);
                return ServiceResult<RevenueStatisticsDto>.Error("Lỗi khi lấy thống kê doanh thu");
            }
        }

        public async Task<ServiceResult<RevenueChartDto>> GetRevenueChartDataAsync(RevenueStatisticsRequestDto request)
        {
            try
            {
                // Validate input
                if (request.FromDate > request.ToDate)
                {
                    return ServiceResult<RevenueChartDto>.Error("Ngày bắt đầu không thể lớn hơn ngày kết thúc");
                }

                if (request.ToDate > DateTime.Now.Date)
                {
                    return ServiceResult<RevenueChartDto>.Error("Ngày kết thúc không thể lớn hơn ngày hiện tại");
                }

                var chartData = await _statisticsRepository.GetRevenueChartDataAsync(request.FromDate, request.ToDate);
                
                return ServiceResult<RevenueChartDto>.Ok("Lấy dữ liệu biểu đồ thành công", chartData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting revenue chart data for period {FromDate} to {ToDate}", 
                    request.FromDate, request.ToDate);
                return ServiceResult<RevenueChartDto>.Error("Lỗi khi lấy dữ liệu biểu đồ doanh thu");
            }
        }

        public async Task<ServiceResult<byte[]>> ExportRevenueReportAsync(RevenueStatisticsRequestDto request)
        {
            try
            {
                // Validate input
                if (request.FromDate > request.ToDate)
                {
                    return ServiceResult<byte[]>.Error("Ngày bắt đầu không thể lớn hơn ngày kết thúc");
                }

                if (request.ToDate > DateTime.Now.Date)
                {
                    return ServiceResult<byte[]>.Error("Ngày kết thúc không thể lớn hơn ngày hiện tại");
                }

                // Lấy dữ liệu thống kê
                var statistics = await _statisticsRepository.GetRevenueStatisticsAsync(request.FromDate, request.ToDate);
                
                if (statistics == null)
                {
                    return ServiceResult<byte[]>.Error("Không có dữ liệu để xuất báo cáo");
                }

                // Tạo file Excel từ dữ liệu thống kê
                var excelData = GenerateExcelReport(statistics);
                
                return ServiceResult<byte[]>.Ok("Xuất báo cáo thành công", excelData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting revenue report for period {FromDate} to {ToDate}", 
                    request.FromDate, request.ToDate);
                return ServiceResult<byte[]>.Error("Lỗi khi xuất báo cáo doanh thu");
            }
        }

        /// <summary>
        /// Tạo file Excel từ dữ liệu thống kê
        /// </summary>
        private byte[] GenerateExcelReport(RevenueStatisticsDto statistics)
        {
            // TODO: Implement proper Excel generation with a library like EPPlus
            // For now, return a simple CSV-like format as bytes
            var csvContent = $"Báo cáo doanh thu từ {statistics.FromDate:dd/MM/yyyy} đến {statistics.ToDate:dd/MM/yyyy}\n";
            csvContent += $"Tổng doanh thu,{statistics.TotalRevenue:N0} VND\n";
            csvContent += $"Tổng lịch khám,{statistics.TotalAppointments}\n";
            csvContent += $"Trung bình mỗi ngày,{statistics.AverageRevenuePerDay:N0} VND\n\n";
            
            csvContent += "Doanh thu theo ngày\n";
            csvContent += "Ngày,Doanh thu,Số lịch khám\n";
            
            foreach (var daily in statistics.DailyRevenues)
            {
                csvContent += $"{daily.Date:dd/MM/yyyy},{daily.TotalRevenue:N0},{daily.AppointmentCount}\n";
            }
            
            return System.Text.Encoding.UTF8.GetBytes(csvContent);
        }
    }
} 