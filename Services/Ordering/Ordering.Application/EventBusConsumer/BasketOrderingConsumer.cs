using EventBus.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Ordering.Application.Abstractions;
using Ordering.Application.Mappers;
using Ordering.Application.Orders.CreateOrder;

namespace Ordering.Application.EventBusConsumer
{
    public class BasketOrderingConsumer : IConsumer<BasketCheckoutEvent>
    {
        private readonly ICommandHandler<CreateOrderCommand, int> _createOrderHandler;
        private readonly ILogger<BasketCheckoutEvent> _logger;

        public BasketOrderingConsumer(ICommandHandler<CreateOrderCommand, int> createOrderHandler,ILogger<BasketCheckoutEvent> logger)
        {
            _createOrderHandler = createOrderHandler;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            using var scope = _logger.BeginScope("Consuming Basket Checkout Event for {CorrelationId}", context.Message.CorrelationId);
            var command = context.Message.ToCheckoutOrderCommand();
            var orderId = await _createOrderHandler.Handle(command, context.CancellationToken);

            _logger.LogInformation("Basket Checkout Event Completed Successfully! OrderId: {OrderId}", orderId);
        }
    }
}
