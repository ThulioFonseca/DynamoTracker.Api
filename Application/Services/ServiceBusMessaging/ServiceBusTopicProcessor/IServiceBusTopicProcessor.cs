namespace Application.Services.ServiceBusMessaging.ServiceBusTopicProcessor
{
    public interface IServiceBusTopicProcessor
    {
        Task PrepareFiltersAndHandleMessages();
        Task CloseSubscriptionAsync();
        ValueTask DisposeAsync();
    }
}
