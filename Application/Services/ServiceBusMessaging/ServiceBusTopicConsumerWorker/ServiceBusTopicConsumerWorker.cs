using Application.Services.ServiceBusMessaging.ServiceBusTopicProcessor;
using Microsoft.Extensions.Logging;

namespace Application.Services.ServiceBusMessaging.ServiceBusTopicConsumerWorker
{
    public class ServiceBusTopicConsumerWorker(IServiceBusTopicProcessor serviceBusTopicSubscription, ILogger<ServiceBusTopicConsumerWorker> logger) : IServiceBusTopicConsumerWorker
    {
        private readonly ILogger<ServiceBusTopicConsumerWorker> _logger = logger;
        private readonly IServiceBusTopicProcessor _serviceBusTopicProcessor = serviceBusTopicSubscription;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ServiceBusTopicConsumerWorker started");
            await _serviceBusTopicProcessor.PrepareFiltersAndHandleMessages().ConfigureAwait(false);
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ServiceBusTopicConsumerWorker stopped");
            await _serviceBusTopicProcessor.CloseSubscriptionAsync().ConfigureAwait(false);
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
                await _serviceBusTopicProcessor.DisposeAsync();
            }
        }
    }
}
