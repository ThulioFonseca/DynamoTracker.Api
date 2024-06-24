namespace Application.Services.ServiceBusMessaging.ServiceBusQueueProcessor
{
    public interface IServiceBusQueueProcessor
    {
        Task HandleMessages();
        Task CloseProcessorAsync();
        ValueTask DisposeAsync();
    }
}
