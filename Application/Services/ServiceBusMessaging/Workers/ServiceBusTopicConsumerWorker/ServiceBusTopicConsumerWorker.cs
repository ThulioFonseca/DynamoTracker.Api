using Application.Services.ServiceBusMessaging.Processors.ServiceBusTopicProcessor;
using Microsoft.Extensions.Logging;

namespace Application.Services.ServiceBusMessaging.Workers.ServiceBusTopicConsumerWorker
{
    public class ServiceBusTopicConsumerWorker(IServiceBusTopicProcessor serviceBusTopicSubscription, ILogger<ServiceBusTopicConsumerWorker> logger) : IServiceBusTopicConsumerWorker
    {
        private readonly ILogger<ServiceBusTopicConsumerWorker> _logger = logger;
        private readonly IServiceBusTopicProcessor _serviceBusTopicProcessor = serviceBusTopicSubscription;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("{worker} started", nameof(ServiceBusTopicConsumerWorker));
                await _serviceBusTopicProcessor.PrepareFiltersAndHandleMessages().ConfigureAwait(false);
            }
            catch (Exception)
            {
                _logger.LogError("Fail to start {worker}.", nameof(ServiceBusTopicConsumerWorker));
            }
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("{worker} stopped", nameof(ServiceBusTopicConsumerWorker));
                await _serviceBusTopicProcessor.CloseSubscriptionAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                _logger.LogError("Fail to stop {worker}.", nameof(ServiceBusTopicConsumerWorker));
            }
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
