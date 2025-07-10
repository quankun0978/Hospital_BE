using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Hospital_BE.DAL.Context;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.PL.DTOs;

namespace Hospital_BE.DAL.Repositories
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly ApplicationDbContext _context;

        public StatisticsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RevenueStatisticsDto> GetRevenueStatisticsAsync(DateTime fromDate, DateTime toDate)
        {
            // Lấy tất cả appointments trong khoảng thời gian và có trạng thái đã khám xong (S3)
            var appointments = await _context.Appointments
                .Where(a => a.AppointmentDate.Date >= fromDate.Date && 
                           a.AppointmentDate.Date <= toDate.Date &&
                           a.Status == "S3") // Chỉ tính appointments đã khám xong
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.DoctorInfos)
                        .ThenInclude(di => di.Price)
                .ToListAsync();

            var dailyRevenues = await GetDailyRevenueAsync(fromDate, toDate);
            var doctorRevenues = await GetDoctorRevenueAsync(fromDate, toDate);

            var totalRevenue = dailyRevenues.Sum(d => d.TotalRevenue);
            var totalAppointments = appointments.Count;
            var daysDiff = (toDate.Date - fromDate.Date).Days + 1;
            var averageRevenuePerDay = daysDiff > 0 ? totalRevenue / daysDiff : 0;

            return new RevenueStatisticsDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                TotalRevenue = totalRevenue,
                TotalAppointments = totalAppointments,
                AverageRevenuePerDay = averageRevenuePerDay,
                DailyRevenues = dailyRevenues,
                TopDoctors = doctorRevenues.OrderByDescending(d => d.Revenue).Take(10).ToList()
            };
        }

        public async Task<List<DailyRevenueDto>> GetDailyRevenueAsync(DateTime fromDate, DateTime toDate)
        {
            var appointments = await _context.Appointments
                .Where(a => a.AppointmentDate.Date >= fromDate.Date && 
                           a.AppointmentDate.Date <= toDate.Date &&
                           a.Status == "S3")
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.DoctorInfos)
                        .ThenInclude(di => di.Price)
                .ToListAsync();

            var dailyData = appointments
                .GroupBy(a => a.AppointmentDate.Date)
                .Select(g => new DailyRevenueDto
                {
                    Date = g.Key,
                    AppointmentCount = g.Count(),
                    TotalRevenue = g.Sum(a => GetConsultationFee(a.Doctor?.DoctorInfos?.FirstOrDefault()?.Price?.ValueVi)),
                    DoctorRevenues = g.GroupBy(a => a.DoctorId)
                        .Select(dg => new DoctorRevenueDto
                        {
                            DoctorId = dg.Key,
                            DoctorName = dg.First().Doctor?.Name ?? "Không xác định",
                            AppointmentCount = dg.Count(),
                            ConsultationFee = GetConsultationFee(dg.First().Doctor?.DoctorInfos?.FirstOrDefault()?.Price?.ValueVi),
                            Revenue = dg.Count() * GetConsultationFee(dg.First().Doctor?.DoctorInfos?.FirstOrDefault()?.Price?.ValueVi)
                        }).ToList()
                })
                .OrderBy(d => d.Date)
                .ToList();

            // Điền vào những ngày không có appointment
            var allDates = new List<DailyRevenueDto>();
            for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
            {
                var existingData = dailyData.FirstOrDefault(d => d.Date == date);
                allDates.Add(existingData ?? new DailyRevenueDto
                {
                    Date = date,
                    TotalRevenue = 0,
                    AppointmentCount = 0,
                    DoctorRevenues = new List<DoctorRevenueDto>()
                });
            }

            return allDates;
        }

        public async Task<List<DoctorRevenueDto>> GetDoctorRevenueAsync(DateTime fromDate, DateTime toDate)
        {
            var appointments = await _context.Appointments
                .Where(a => a.AppointmentDate.Date >= fromDate.Date && 
                           a.AppointmentDate.Date <= toDate.Date &&
                           a.Status == "S3")
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.DoctorInfos)
                        .ThenInclude(di => di.Price)
                .ToListAsync();

            var doctorRevenues = appointments
                .GroupBy(a => a.DoctorId)
                .Select(g => new DoctorRevenueDto
                {
                    DoctorId = g.Key,
                    DoctorName = g.First().Doctor?.Name ?? "Không xác định",
                    AppointmentCount = g.Count(),
                    ConsultationFee = GetConsultationFee(g.First().Doctor?.DoctorInfos?.FirstOrDefault()?.Price?.ValueVi),
                    Revenue = g.Count() * GetConsultationFee(g.First().Doctor?.DoctorInfos?.FirstOrDefault()?.Price?.ValueVi)
                })
                .OrderByDescending(d => d.Revenue)
                .ToList();

            return doctorRevenues;
        }

        public async Task<RevenueChartDto> GetRevenueChartDataAsync(DateTime fromDate, DateTime toDate)
        {
            var dailyRevenues = await GetDailyRevenueAsync(fromDate, toDate);

            return new RevenueChartDto
            {
                Labels = dailyRevenues.Select(d => d.Date.ToString("dd/MM")).ToList(),
                Data = dailyRevenues.Select(d => d.TotalRevenue).ToList(),
                AppointmentCounts = dailyRevenues.Select(d => d.AppointmentCount).ToList()
            };
        }

        /// <summary>
        /// Chuyển đổi chuỗi giá tiền thành decimal
        /// </summary>
        private decimal GetConsultationFee(string priceValue)
        {
            if (string.IsNullOrEmpty(priceValue))
                return 0;

            // Loại bỏ các ký tự không phải số, chỉ giữ lại số và dấu phẩy, chấm
            var cleanedValue = new string(priceValue.Where(c => char.IsDigit(c) || c == ',' || c == '.').ToArray());
            
            if (string.IsNullOrEmpty(cleanedValue))
                return 0;

            // Xử lý format số tiền Việt Nam: 500,000 hoặc 500.000
            // Nếu có dấu phẩy hoặc chấm, coi như là phân cách hàng nghìn
            if (cleanedValue.Contains(',') || cleanedValue.Contains('.'))
            {
                // Loại bỏ dấu phẩy và chấm (coi như separator hàng nghìn)
                var numericValue = cleanedValue.Replace(",", "").Replace(".", "");
                if (decimal.TryParse(numericValue, out decimal result))
                    return result;
            }
            else
            {
                // Nếu không có dấu phẩy hoặc chấm, parse trực tiếp
                if (decimal.TryParse(cleanedValue, out decimal result))
                    return result;
            }

            return 0;
        }
    }
} 