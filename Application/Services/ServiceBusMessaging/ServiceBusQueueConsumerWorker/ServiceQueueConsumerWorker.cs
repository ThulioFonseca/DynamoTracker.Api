using Application.Services.ServiceBusMessaging.ServiceBusQueueProcessor;
using Microsoft.Extensions.Logging;

namespace Application.Services.ServiceBusMessaging.ServiceBusQueueConsumerWorker
{
    public class ServiceQueueConsumerWorker(ILogger<ServiceQueueConsumerWorker> logger,
        IServiceBusQueueProcessor serviceBusQueueProcessor) : IServiceBusQueueConsumerWorker
    {
        private readonly ILogger _logger = logger;
        private readonly IServiceBusQueueProcessor _serviceBusQueueProcessor = serviceBusQueueProcessor;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ServiceQueueConsumerWorker started");
            await _serviceBusQueueProcessor.HandleMessages().ConfigureAwait(false);
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ServiceBusTopicConsumerWorker stopped");
            await _serviceBusQueueProcessor.CloseProcessorAsync().ConfigureAwait(false);
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
