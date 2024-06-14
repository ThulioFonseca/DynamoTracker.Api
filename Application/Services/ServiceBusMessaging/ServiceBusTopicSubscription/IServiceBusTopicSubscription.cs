namespace Application.Services.ServiceBusMessaging.ServiceBusTopicSubscription
{
    public interface IServiceBusTopicSubscription
    {
        Task PrepareFiltersAndHandleMessages();
        Task CloseSubscriptionAsync();
        ValueTask DisposeAsync();
    }
}
