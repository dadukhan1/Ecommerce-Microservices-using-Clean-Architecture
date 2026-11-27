using EventBus.Messages.Common;
using MassTransit;
using Ordering.API.Extensions;
using Ordering.Application.Dispatchers;
using Ordering.Application.EventBusConsumer;
using Ordering.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Ordering Services
builder.Services.AddOrderingService(builder.Configuration);

// Register outbox message dispatcher 
builder.Services.AddHostedService<OutboxMessageDispatcher>();

// Mass Transit
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<BasketOrderingConsumer>();
    config.AddConsumer<PaymentCompletedConsumer>();
    config.AddConsumer<PaymentFailedConsumer>();

    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        cfg.ReceiveEndpoint(EventBusConstant.BasketCheckoutQueue, c =>
        {
            c.ConfigureConsumer<BasketOrderingConsumer>(ctx);
        });
        cfg.ReceiveEndpoint(EventBusConstant.PaymentCompletedQueue, c =>
        {
            c.ConfigureConsumer<PaymentCompletedConsumer>(ctx);
        });
        cfg.ReceiveEndpoint(EventBusConstant.PaymentFailedQueue, c =>
        {
            c.ConfigureConsumer<PaymentFailedConsumer>(ctx);
        });
    });
});

var app = builder.Build();

// Migration 
app.MigrateDatabase<OrderContext>((context, services) =>
{
    var logger = services.GetService<ILogger<OrderContextSeed>>();
    OrderContextSeed.SeedAsync(context, logger).Wait();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
