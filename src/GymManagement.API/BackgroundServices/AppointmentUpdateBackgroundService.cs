

using GymManagement.Application.Interfaces.Services.User;

namespace GymManagement.API.BackgroundServices
{
    /// <summary>
    /// Background service để tự động cập nhật appointments đã missed
    /// Chạy hàng ngày lúc 00:05
    /// </summary>
    public class AppointmentUpdateBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AppointmentUpdateBackgroundService> _logger;

        public AppointmentUpdateBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<AppointmentUpdateBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("✅ Appointment Update Background Service đã khởi động");

            // Lên lịch chạy hàng ngày
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var nextRun = DateTime.Today.AddDays(1).AddHours(0).AddMinutes(5); // 00:05 ngày hôm sau
                var delay = nextRun - now;

                if (delay.TotalMilliseconds > 0)
                {
                    _logger.LogInformation($"⏰ Lần chạy tiếp theo: {nextRun:yyyy-MM-dd HH:mm:ss} UTC");
                    await Task.Delay(delay, stoppingToken);
                }

                await UpdateMissedAppointments();
            }
        }

        private async Task UpdateMissedAppointments()
        {
            try
            {
                _logger.LogInformation("🔁 Đang cập nhật appointments đã missed...");

                using var scope = _serviceProvider.CreateScope();
                var appointmentService = scope.ServiceProvider.GetRequiredService<IAppointmentService>();

                await appointmentService.UpdateMissedAppointmentsAsync();

                _logger.LogInformation("✅ Hoàn thành cập nhật appointments đã missed");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi khi cập nhật appointments đã missed: {ex.Message}");
            }
        }
    }
}