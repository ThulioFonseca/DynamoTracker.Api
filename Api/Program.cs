using Application.Services.ServiceBusMessaging.ServiceBusQueueConsumerWorker;
using Application.Services.ServiceBusMessaging.ServiceBusQueueProcessor;
using Application.Services.ServiceBusMessaging.ServiceBusTopicConsumerWorker;
using Application.Services.ServiceBusMessaging.ServiceBusTopicProcessor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IServiceBusTopicProcessor, ServiceBusTopicProcessor>();
builder.Services.AddSingleton<IServiceBusQueueProcessor, ServiceBusQueueProcessor>();

builder.Services.AddHostedService<ServiceBusTopicConsumerWorker>();
builder.Services.AddHostedService<ServiceQueueConsumerWorker>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
