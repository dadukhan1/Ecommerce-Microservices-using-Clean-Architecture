using Microsoft.Extensions.Logging;
using Ordering.Application.Abstractions;
using Ordering.Application.Mappers;
using Ordering.Core.Repositories;

namespace Ordering.Application.Orders.CreateOrder
{
    public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, int>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<CreateOrderCommandHandler> _logger;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, ILogger<CreateOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<int> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var orderEntity = command.ToEntity();
            var genereatedOrder = await _orderRepository.AddAsync(orderEntity);
            var outboxMessage = OrderMapper.ToOutboxMessage(genereatedOrder, command.CorrelationId);
            await _orderRepository.AddOutboxMessageAsync(outboxMessage);
            _logger.LogInformation($"Order with ID {genereatedOrder.Id} successfully created.");
            return genereatedOrder.Id;
        }
    }
}
