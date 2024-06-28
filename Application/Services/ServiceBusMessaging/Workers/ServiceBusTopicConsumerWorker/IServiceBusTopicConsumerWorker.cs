using Microsoft.Extensions.Hosting;

namespace Application.Services.ServiceBusMessaging.Workers.ServiceBusTopicConsumerWorker
{
    public interface IServiceBusTopicConsumerWorker : IHostedService, IDisposable
    {
    }
}
