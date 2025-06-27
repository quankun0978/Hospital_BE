using System;
using System.Linq;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.DAL.Context;
using Microsoft.EntityFrameworkCore;

namespace Hospital_BE.BLL.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy thống kê dashboard cho admin
        /// </summary>
        /// <returns>Thống kê dashboard</returns>
        public async Task<object> GetDashboardStatsAsync()
        {
            try
            {
                var today = DateTime.Today;
                
                // Tổng số người dùng
                var totalUsers = await _context.Users.CountAsync();
                
                // Lịch hẹn hôm nay
                var todayAppointments = await _context.Appointments
                    .Where(a => a.AppointmentDate.Date == today)
                    .CountAsync();
                
                // Bệnh nhân mới đăng ký trong ngày (R3 là role bệnh nhân)
                // Vì User không có CreatedAt, ta sẽ dùng PatientRecord
                var newPatients = await _context.PatientRecords
                    .Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Date == today)
                    .CountAsync();
                
                // Lịch hẹn đã khám xong hôm nay (status S3 theo Allcode)
                var completedAppointments = await _context.Appointments
                    .Where(a => a.Status == "S3" && a.AppointmentDate.Date == today)
                    .CountAsync();

                return new
                {
                    TotalUsers = totalUsers,
                    TodayAppointments = todayAppointments,
                    NewPatients = newPatients,
                    PendingAppointments = completedAppointments
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê dashboard: {ex.Message}");
            }
        }
    }
} 