using Microsoft.Extensions.Hosting;

namespace Application.Services.ServiceBusMessaging.Workers.ServiceBusQueueConsumerWorker
{
    public interface IServiceBusQueueConsumerWorker : IHostedService, IDisposable
    {
    }
}
