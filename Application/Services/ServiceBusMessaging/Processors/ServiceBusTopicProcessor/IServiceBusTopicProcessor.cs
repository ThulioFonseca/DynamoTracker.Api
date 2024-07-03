namespace Application.Services.ServiceBusMessaging.Processors.ServiceBusTopicProcessor
{
    public interface IServiceBusTopicProcessor
    {
        Task PrepareFiltersAndHandleMessages();
        Task CloseSubscriptionAsync();
        ValueTask DisposeAsync();
    }
}
