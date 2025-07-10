using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.PL.Controllers.Base;
using Hospital_BE.PL.DTOs;
using Hospital_BE.PL.DTOs.Common;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Hospital_BE.PL.Controllers
{
    /// <summary>
    /// Controller xử lý các API liên quan đến thống kê
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : BaseController
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        /// <summary>
        /// Lấy thống kê doanh thu theo khoảng thời gian
        /// </summary>
        /// <param name="request">Thông tin khoảng thời gian thống kê</param>
        /// <returns>Thống kê doanh thu chi tiết</returns>
        [HttpPost("revenue")]
        public async Task<IActionResult> GetRevenueStatistics([FromBody] RevenueStatisticsRequestDto request)
        {
            try
            {
                var result = await _statisticsService.GetRevenueStatisticsAsync(request);
                
                if (result.Success)
                {
                    return ApiOk(result.Data, result.Message);
                }
                
                return ApiBadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return ApiBadRequest($"Lỗi khi lấy thống kê doanh thu: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy dữ liệu biểu đồ doanh thu
        /// </summary>
        /// <param name="request">Thông tin khoảng thời gian thống kê</param>
        /// <returns>Dữ liệu cho biểu đồ</returns>
        [HttpPost("revenue/chart")]
        public async Task<IActionResult> GetRevenueChartData([FromBody] RevenueStatisticsRequestDto request)
        {
            try
            {
                var result = await _statisticsService.GetRevenueChartDataAsync(request);
                
                if (result.Success)
                {
                    return ApiOk(result.Data, result.Message);
                }
                
                return ApiBadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return ApiBadRequest($"Lỗi khi lấy dữ liệu biểu đồ: {ex.Message}");
            }
        }

        /// <summary>
        /// Xuất báo cáo doanh thu ra file Excel
        /// </summary>
        /// <param name="request">Thông tin khoảng thời gian thống kê</param>
        /// <returns>File Excel báo cáo</returns>
        [HttpPost("revenue/export")]
        public async Task<IActionResult> ExportRevenueReport([FromBody] RevenueStatisticsRequestDto request)
        {
            try
            {
                var result = await _statisticsService.ExportRevenueReportAsync(request);
                
                if (result.Success)
                {
                    var fileName = $"BaoCaoDoanhThu_{request.FromDate:yyyyMMdd}_{request.ToDate:yyyyMMdd}.csv";
                    return File(result.Data, "text/csv;charset=utf-8;", fileName);
                }
                
                return ApiBadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return ApiBadRequest($"Lỗi khi xuất báo cáo: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy thống kê tổng quan theo khoảng thời gian (cho dashboard)
        /// </summary>
        /// <param name="fromDate">Ngày bắt đầu</param>
        /// <param name="toDate">Ngày kết thúc</param>
        /// <returns>Thống kê tổng quan</returns>
        [HttpGet("overview")]
        public async Task<IActionResult> GetStatisticsOverview(
            [FromQuery] DateTime? fromDate = null, 
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                // Nếu không truyền tham số, mặc định lấy 30 ngày gần nhất
                var from = fromDate ?? DateTime.Now.AddDays(-30).Date;
                var to = toDate ?? DateTime.Now.Date;

                var request = new RevenueStatisticsRequestDto
                {
                    FromDate = from,
                    ToDate = to
                };

                var result = await _statisticsService.GetRevenueStatisticsAsync(request);
                
                if (result.Success)
                {
                    // Trả về thông tin tổng quan
                    var overview = new
                    {
                        TotalRevenue = result.Data.TotalRevenue,
                        TotalAppointments = result.Data.TotalAppointments,
                        AverageRevenuePerDay = result.Data.AverageRevenuePerDay,
                        TopDoctors = result.Data.TopDoctors.Take(5), // Top 5 bác sĩ
                        Period = new { From = from, To = to }
                    };

                    return ApiOk(overview, "Lấy thống kê tổng quan thành công");
                }
                
                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return ApiBadRequest($"Lỗi khi lấy thống kê tổng quan: {ex.Message}");
            }
        }
    }
} 