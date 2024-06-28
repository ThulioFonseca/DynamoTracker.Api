using Application.Services.ServiceBusMessaging.Processors.ServiceBusQueueProcessor;
using Microsoft.Extensions.Logging;

namespace Application.Services.ServiceBusMessaging.Workers.ServiceBusQueueConsumerWorker
{
    public class ServiceQueueConsumerWorker(ILogger<ServiceQueueConsumerWorker> logger,
        IServiceBusQueueProcessor serviceBusQueueProcessor) : IServiceBusQueueConsumerWorker
    {
        private readonly ILogger _logger = logger;
        private readonly IServiceBusQueueProcessor _serviceBusQueueProcessor = serviceBusQueueProcessor;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("{worker} started", nameof(ServiceQueueConsumerWorker));
                await _serviceBusQueueProcessor.HandleMessages().ConfigureAwait(false);
            }
            catch (Exception)
            {
                _logger.LogError("Fail to start {worker}.", nameof(ServiceQueueConsumerWorker));
            }
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("{worker} stopped", nameof(ServiceQueueConsumerWorker));
                await _serviceBusQueueProcessor.CloseProcessorAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                _logger.LogError("Fail to stop {worker}.", nameof(ServiceQueueConsumerWorker));
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
                await _serviceBusQueueProcessor.DisposeAsync();
            }
        }
    }
}
