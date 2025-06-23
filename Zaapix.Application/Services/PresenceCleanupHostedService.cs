using Microsoft.Extensions.Hosting;

namespace Zaapix.Application.Services
{
    public class PresenceCleanupHostedService : BackgroundService
    {
        private readonly IPresenceService _presenceService;

        public PresenceCleanupHostedService(IPresenceService presenceService)
        {
            _presenceService = presenceService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _presenceService.Cleanup();
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}