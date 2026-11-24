using Microsoft.Extensions.Logging;
using Ordering.Application.Abstractions;
using Ordering.Application.Exceptions;
using Ordering.Application.Mappers;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;

namespace Ordering.Application.Orders.UpdateOrder
{
    public class UpdateOrderCommandHandler : ICommandHandler<UpdateOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<UpdateOrderCommand> _logger;

        public UpdateOrderCommandHandler(IOrderRepository orderRepository, ILogger<UpdateOrderCommand> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }
        public async Task Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _orderRepository.GetByIdAsync(command.Id);
            if (orderToUpdate == null)
            {
                throw new OrderNotFoundException(nameof(Order), command.Id);
            }

            // Map existing field
            orderToUpdate.ApplyUpdate(command);

            await _orderRepository.UpdateAsync(orderToUpdate);
            _logger.LogInformation($"Order {command.Id} is successfully updated.");
        }
    }
}
