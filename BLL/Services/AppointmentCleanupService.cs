using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Hospital_BE.DAL.Context;
using Microsoft.EntityFrameworkCore;

namespace Hospital_BE.BLL.Services
{
    public class AppointmentCleanupService : BackgroundService
    {
        private readonly ILogger<AppointmentCleanupService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public AppointmentCleanupService(
            ILogger<AppointmentCleanupService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupUnconfirmedAppointments();
                    
                    // Chạy mỗi 1 phút
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi thực thi cleanup appointments");
                    // Nếu có lỗi, đợi 5 phút trước khi thử lại
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }

        private async Task CleanupUnconfirmedAppointments()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                // Tìm các lịch hẹn S1 (chưa xác nhận) đã quá 10 phút
                var cutoffTime = DateTime.Now.AddMinutes(-10);
                
                var expiredAppointments = await context.Appointments
                    .Where(a => a.Status == "S1" && a.CreatedAt < cutoffTime)
                    .ToListAsync();

                if (expiredAppointments.Any())
                {
                    _logger.LogInformation($"Tìm thấy {expiredAppointments.Count} lịch hẹn chưa xác nhận đã hết hạn");

                    // Xóa các lịch hẹn đã hết hạn
                    context.Appointments.RemoveRange(expiredAppointments);
                    await context.SaveChangesAsync();

                    // Log thông tin chi tiết
                    foreach (var appointment in expiredAppointments)
                    {
                        _logger.LogInformation($"Đã xóa lịch hẹn ID: {appointment.AppointmentId}, tạo lúc: {appointment.CreatedAt}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cleanup appointments từ database");
            }
        }
    }
} 