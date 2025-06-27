using System;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.PL.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_BE.PL.Controllers
{
    public class AdminController : BaseController
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
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
                var stats = await _adminService.GetDashboardStatsAsync();
                return ApiOk(stats, "Lấy thống kê dashboard thành công");
            }
            catch (Exception ex)
            {
                return ApiBadRequest<object>($"Lỗi khi lấy thống kê: {ex.Message}");
            }
        }
    }
} 