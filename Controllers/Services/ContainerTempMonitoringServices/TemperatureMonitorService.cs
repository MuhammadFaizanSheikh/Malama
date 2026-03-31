using ExcelFilesCompiler.UnitOfWork;
using Malama.Models;
using Microsoft.AspNetCore.SignalR;

namespace Malama.Controllers.Services.ContainerTempMonitoringServices
{
    public class TemperatureMonitorService : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly IHubContext<TemperatureHub> _hub;

        public TemperatureMonitorService(IServiceProvider provider, IHubContext<TemperatureHub> hub)
        {
            _provider = provider;
            _hub = hub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAlertsAsync();
                }
                catch (Exception ex)
                {
                    // log exception if you have a logger
                    Console.WriteLine($"TemperatureMonitorService Error: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            }
        }

        private async Task CheckAlertsAsync()
        {
            using var scope = _provider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var now = DateTime.Now;

            // Get containers where temperature check is due
            var dueContainers = unitOfWork.Container
                .GetAllWithConditionNoTracking(c => c.NextExpectedReadingUtc <= now && c.FinalTemp  == false);

            foreach (var container in dueContainers)
            {
                //var userId = container.AddedBy;

                // Check if a notification already exists for this monitoring attempt
                var existingNotification = await unitOfWork.ContainerNotification
                    .GetFirstOrDefaultWithConditionNoTracking(n => n.ContainerId == container.Id && n.DueAt == container.NextExpectedReadingUtc);

                ContainerNotification notification;

                if (existingNotification == null)
                {
                    // No notification exists yet — create a new one
                    notification = new ContainerNotification
                    {
                        ContainerId = container.Id,
                        //UserId = userId,
                        DueAt = container.NextExpectedReadingUtc,
                        IsAcknowledged = false,
                        AddedBy = "Background Service",
                        AddedOn = DateTime.Now
                    };

                    await unitOfWork.ContainerNotification.AddAsync(notification);
                    await unitOfWork.SaveAsync();
                }
                else
                {
                    // Use the existing notification
                    notification = existingNotification;
                }

                // Send SignalR alert **only if not acknowledged**
                if (!notification.IsAcknowledged)
                {
                    await _hub.Clients.Group(container.EventManagementId.ToString()).SendAsync("TemperatureAlert", new
                    {
                        notificationId = notification.Id,
                        containerId = container.Id,
                        containerName = container.ContainerName,
                        message = $"Temperature reading required for {container.ContainerName}",
                        dueAt = container.NextExpectedReadingUtc.ToString("MM/dd/yyyy HH:mm")
                    });
                }
            }
        }


    }
}
