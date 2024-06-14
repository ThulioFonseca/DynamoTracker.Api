using Application.Services.ServiceBusMessaging.ServiceBusTopicSubscription;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Services.ServiceBusMessaging.ServiceBusConsumerWorker
{
    public class ServiceBusConsumerWorker(IServiceBusTopicSubscription serviceBusTopicSubscription, ILogger<ServiceBusConsumerWorker> logger) : IHostedService, IDisposable
    {
        private readonly IServiceBusTopicSubscription _serviceBusTopicSubscription = serviceBusTopicSubscription;
        private readonly ILogger<ServiceBusConsumerWorker> _logger = logger;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ServiceBusConsumerWorker started");
            await _serviceBusTopicSubscription.PrepareFiltersAndHandleMessages().ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ServiceBusConsumerWorker stopped");
            await _serviceBusTopicSubscription.CloseSubscriptionAsync().ConfigureAwait(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async void Dispose(bool disposing)
        {
            if (disposing)
            {
                await _serviceBusTopicSubscription.DisposeAsync();
            }
        }

    }
}
