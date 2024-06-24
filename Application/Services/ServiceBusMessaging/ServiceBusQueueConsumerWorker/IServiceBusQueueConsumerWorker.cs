using Microsoft.Extensions.Hosting;

namespace Application.Services.ServiceBusMessaging.ServiceBusQueueConsumerWorker
{
    public interface IServiceBusQueueConsumerWorker : IHostedService, IDisposable
    {        
    }
}
