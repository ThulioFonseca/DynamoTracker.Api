namespace Application.Services.ServiceBusMessaging.Processors.ServiceBusQueueProcessor
{
    public interface IServiceBusQueueProcessor
    {
        Task HandleMessages();
        Task CloseProcessorAsync();
        ValueTask DisposeAsync();
    }
}
