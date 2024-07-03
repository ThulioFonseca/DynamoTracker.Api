using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Services.ServiceBusMessaging.Processors.ServiceBusTopicProcessor
{
    public class ServiceBusTopicProcessor : IServiceBusTopicProcessor
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly ServiceBusClient _client;
        private readonly ServiceBusAdministrationClient _adminClient;
        private ServiceBusProcessor _processor;

        public ServiceBusTopicProcessor(
            IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
            ServiceBusAdministrationClient serviceBusAdministrationClient,
            IConfiguration configuration,
            ILogger<ServiceBusTopicProcessor> logger)
        {
            var connectionString = _configuration.GetConnectionString("ServiceBus");

            _configuration = configuration;
            _logger = logger;
            _client = serviceBusClientFactory.CreateClient("service-bus-client");
            _adminClient = serviceBusAdministrationClient;
        }

        public async Task PrepareFiltersAndHandleMessages()
        {
            ServiceBusProcessorOptions _serviceBusProcessorOptions = new()
            {
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = false,
            };

            var topic = _configuration["ServiceBus:TopicName"];
            var subscription = _configuration["ServiceBus:SubscriptionName"];

            try
            {
                _processor = _client.CreateProcessor(topic, subscription, _serviceBusProcessorOptions);
                _processor.ProcessMessageAsync += ProcessMessagesAsync;
                _processor.ProcessErrorAsync += ProcessErrorAsync;

                await ConfigureDefaultFilters();

                await _processor.StartProcessingAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred: {ex_Name}. Details: {ex_Message}", ex.GetType().Name, ex.Message);
            }
        }
        private async Task ConfigureDefaultFilters()
        {
            try
            {
                var rules = _adminClient.GetRulesAsync(_configuration["ServiceBus:TopicName"], _configuration["ServiceBus:SubscriptionName"]);
                var ruleProperties = new List<RuleProperties>();

                await foreach (var rule in rules)
                    ruleProperties.Add(rule);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Fail to configure filters: {ex_Name}. Details: {ex_Message}", ex.GetType().Name, ex.Message);
            }
        }
        private async Task ProcessMessagesAsync(ProcessMessageEventArgs args)
        {
            //var myPayload = args.Message.Body.ToObjectFromJson<Message>();
            var message = args.Message.Body.ToString();

            _logger.LogInformation("Received message: {myPayload}", message);
            //await _processData.Process(myPayload).ConfigureAwait(false);
            await args.CompleteMessageAsync(args.Message).ConfigureAwait(false);
        }
        private Task ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            _logger.LogError("Message handler encountered an exception: {ex}", arg.Exception);
            _logger.LogDebug("- ErrorSource: {ErrorSource}", arg.ErrorSource);
            _logger.LogDebug("- Entity Path: {EntityPath}", arg.EntityPath);
            _logger.LogDebug("- FullyQualifiedNamespace: {FullyQualifiedNamespace}", arg.FullyQualifiedNamespace);

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
        public async Task CloseSubscriptionAsync()
        {
            await _processor!.CloseAsync().ConfigureAwait(false);
        }
    }
}
