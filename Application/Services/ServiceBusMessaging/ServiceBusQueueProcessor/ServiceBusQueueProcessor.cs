using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Services.ServiceBusMessaging.ServiceBusQueueProcessor
{
    public class ServiceBusQueueProcessor : IServiceBusQueueProcessor
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly ServiceBusClient _client;
        private ServiceBusProcessor _processor;

        public ServiceBusQueueProcessor(
            IConfiguration configuration,
            ILogger<ServiceBusQueueProcessor> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var connectionString = _configuration["ServiceBus:ConnectionString"];

            ServiceBusClientOptions clientOptions = new()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };

            _client = new ServiceBusClient(connectionString, clientOptions);
        }

        public async Task HandleMessages()
        {

            ServiceBusProcessorOptions options = new()
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 1,
            };

            var queueName = _configuration["ServiceBus:QueueName"];

            try
            {
                _processor = _client.CreateProcessor(queueName, options);
                _processor.ProcessMessageAsync += ProcessMessagesAsync;
                _processor.ProcessErrorAsync += ProcessErrorAsync;

                await _processor.StartProcessingAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred: {ex.GetType().Name}. Details: {ex.Message}");
            }
        }
        private async Task ProcessMessagesAsync(ProcessMessageEventArgs args)
        {
            //var myPayload = args.Message.Body.ToObjectFromJson<Message>();
            var myPayload = args.Message.Body.ToString();

            _logger.LogInformation("Received message: {myPayload}", myPayload);
            //await _processData.Process(myPayload).ConfigureAwait(false);
            await args.CompleteMessageAsync(args.Message).ConfigureAwait(false);
        }
        private Task ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            _logger.LogError(arg.Exception, "Message handler encountered an exception");
            _logger.LogDebug($"- ErrorSource: {arg.ErrorSource}");
            _logger.LogDebug($"- Entity Path: {arg.EntityPath}");
            _logger.LogDebug($"- FullyQualifiedNamespace: {arg.FullyQualifiedNamespace}");

            return Task.CompletedTask;
        }
        public async ValueTask DisposeAsync()
        {
            if (_processor != null)
            {
                await _processor.DisposeAsync().ConfigureAwait(false);
            }

            if (_client != null)
            {
                await _client.DisposeAsync().ConfigureAwait(false);
            }
        }
        public async Task CloseProcessorAsync()
        {
            await _processor!.CloseAsync().ConfigureAwait(false);

        }
    }
}
