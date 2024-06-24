using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Services.ServiceBusMessaging.ServiceBusTopicProcessor
{
    public class ServiceBusTopicProcessor : IServiceBusTopicProcessor
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly ServiceBusClient _client;
        private readonly ServiceBusAdministrationClient _adminClient;
        private ServiceBusProcessor _processor;

        public ServiceBusTopicProcessor(
            IConfiguration configuration,
            ILogger<ServiceBusTopicProcessor> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var connectionString = _configuration["ServiceBus:ConnectionString"];
            _client = new ServiceBusClient(connectionString);
            _adminClient = new ServiceBusAdministrationClient(connectionString);
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

                await RemoveDefaultFilters();
                await AddFilters();

                await _processor.StartProcessingAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred: {ex.GetType().Name}. Details: {ex.Message}");
            }
        }
        private async Task RemoveDefaultFilters()
        {
            try
            {
                var rules = _adminClient.GetRulesAsync(_configuration["ServiceBus:TopicName"], _configuration["ServiceBus:SubscriptionName"]);
                var ruleProperties = new List<RuleProperties>();
                await foreach (var rule in rules)
                {
                    ruleProperties.Add(rule);
                }

                //foreach (var rule in ruleProperties)
                //{
                //    if (rule.Name == "GoalsGreaterThanSeven")
                //    {
                //        await _adminClient.DeleteRuleAsync(TOPIC_PATH, SUBSCRIPTION_NAME, "GoalsGreaterThanSeven")
                //            .ConfigureAwait(false);
                //    }
                //}
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.ToString());
            }
        }
        private async Task AddFilters()
        {
            try
            {
                var rules = _adminClient.GetRulesAsync(_configuration["ServiceBus:TopicName"], _configuration["ServiceBus:SubscriptionName"])
                    .ConfigureAwait(false);

                var ruleProperties = new List<RuleProperties>();
                await foreach (var rule in rules)
                {
                    ruleProperties.Add(rule);
                }

                //if (!ruleProperties.Any(r => r.Name == "GoalsGreaterThanSeven"))
                //{
                //    CreateRuleOptions createRuleOptions = new CreateRuleOptions
                //    {
                //        Name = "GoalsGreaterThanSeven",
                //        Filter = new SqlRuleFilter("goals > 7")
                //    };
                //    await _adminClient.CreateRuleAsync(TOPIC_PATH, SUBSCRIPTION_NAME, createRuleOptions)
                //        .ConfigureAwait(false);
                //}
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.ToString());
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
        public async Task CloseSubscriptionAsync()
        {
            await _processor!.CloseAsync().ConfigureAwait(false);
        }
    }
}
