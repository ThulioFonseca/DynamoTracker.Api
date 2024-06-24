using Microsoft.Extensions.Hosting;

namespace Application.Services.ServiceBusMessaging.ServiceBusTopicConsumerWorker
{
    public interface IServiceBusTopicConsumerWorker : IHostedService, IDisposable
    {
    }
}
