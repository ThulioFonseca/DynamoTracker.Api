using Application.Services.ServiceBusMessaging.Processors.ServiceBusQueueProcessor;
using Application.Services.ServiceBusMessaging.Processors.ServiceBusTopicProcessor;
using Application.Services.ServiceBusMessaging.Workers.ServiceBusQueueConsumerWorker;
using Application.Services.ServiceBusMessaging.Workers.ServiceBusTopicConsumerWorker;
using Serilog;

namespace Api
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(Log.Logger);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IServiceBusTopicProcessor, ServiceBusTopicProcessor>();
            builder.Services.AddSingleton<IServiceBusQueueProcessor, ServiceBusQueueProcessor>();

            //builder.Services.AddHostedService<ServiceBusTopicConsumerWorker>();
            builder.Services.AddHostedService<ServiceQueueConsumerWorker>();


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}