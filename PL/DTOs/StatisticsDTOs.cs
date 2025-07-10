using System;
using System.Collections.Generic;

namespace Hospital_BE.PL.DTOs
{
    /// <summary>
    /// DTO cho request thống kê doanh thu
    /// </summary>
    public class RevenueStatisticsRequestDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    /// <summary>
    /// DTO cho response thống kê doanh thu theo ngày
    /// </summary>
    public class DailyRevenueDto
    {
        public DateTime Date { get; set; }
        public decimal TotalRevenue { get; set; }
        public int AppointmentCount { get; set; }
        public List<DoctorRevenueDto> DoctorRevenues { get; set; } = new List<DoctorRevenueDto>();
    }

    /// <summary>
    /// DTO cho doanh thu của từng bác sĩ
    /// </summary>
    public class DoctorRevenueDto
    {
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; }
        public decimal Revenue { get; set; }
        public int AppointmentCount { get; set; }
        public decimal ConsultationFee { get; set; }
    }

    /// <summary>
    /// DTO cho tổng hợp thống kê doanh thu
    /// </summary>
    public class RevenueStatisticsDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalAppointments { get; set; }
        public decimal AverageRevenuePerDay { get; set; }
        public List<DailyRevenueDto> DailyRevenues { get; set; } = new List<DailyRevenueDto>();
        public List<DoctorRevenueDto> TopDoctors { get; set; } = new List<DoctorRevenueDto>();
    }

    /// <summary>
    /// DTO cho biểu đồ doanh thu
    /// </summary>
    public class RevenueChartDto
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<decimal> Data { get; set; } = new List<decimal>();
        public List<int> AppointmentCounts { get; set; } = new List<int>();
    }
} 