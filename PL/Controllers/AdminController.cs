using System;
using System.Linq;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.PL.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hospital_BE.DAL.Context;

namespace Hospital_BE.PL.Controllers
{
    public class AdminController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public AdminController(
            ApplicationDbContext context,
            IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        /// <summary>
        /// Lấy thống kê dashboard cho admin
        /// </summary>
        /// <returns>Thống kê dashboard</returns>
        [HttpGet("dashboard-stats")]
        public async Task<IActionResult> GetDashboardStats()
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
                
                // Lịch hẹn chờ xử lý (status S1 theo Allcode)
                var pendingAppointments = await _context.Appointments
                    .Where(a => a.Status == "S")
                    .CountAsync();

                var stats = new
                {
                    TotalUsers = totalUsers,
                    TodayAppointments = todayAppointments,
                    NewPatients = newPatients,
                    PendingAppointments = pendingAppointments
                };

                return ApiOk(stats, "Lấy thống kê dashboard thành công");
            }
            catch (Exception ex)
            {
                return ApiBadRequest<object>($"Lỗi khi lấy thống kê: {ex.Message}");
            }
        }
    }
} 