

using GymManagement.Application.Interfaces.Repositories.User;

namespace GymManagement.API.BackgroundServices
{
    public class WorkoutScheduleBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WorkoutScheduleBackgroundService> _logger;

        public WorkoutScheduleBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<WorkoutScheduleBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Workout Schedule Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Chạy vào 00:05 mỗi ngày
                    var now = DateTime.Now;
                    var nextRun = now.Date.AddDays(1).AddMinutes(5);
                    var delay = nextRun - now;

                    if (delay.TotalMilliseconds > 0)
                    {
                        await Task.Delay(delay, stoppingToken);
                    }

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var repository = scope.ServiceProvider
                            .GetRequiredService<IWorkoutScheduleRepository>();

                        var count = await repository.UpdateMissedSchedulesAsync(DateTime.UtcNow);

                        _logger.LogInformation(
                            "Updated {Count} missed workout schedules at {Time}",
                            count,
                            DateTime.Now);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating missed workout schedules.");
                }
            }
        }
    }
}

// Đăng ký trong Program.cs:
// builder.Services.AddHostedService<WorkoutScheduleBackgroundService>();