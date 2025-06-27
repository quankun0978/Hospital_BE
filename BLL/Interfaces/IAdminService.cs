using System.Threading.Tasks;

namespace Hospital_BE.BLL.Interfaces
{
    public interface IAdminService
    {
        /// <summary>
        /// Lấy thống kê dashboard cho admin
        /// </summary>
        /// <returns>Thống kê dashboard</returns>
        Task<object> GetDashboardStatsAsync();
    }
} 