using Ordering.Application.Abstractions;
using Ordering.Application.Mappers;
using Ordering.Core.Repositories;

namespace Ordering.Application.Orders.CreateOrder
{
    public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, int>
    {
        private readonly IOrderRepository _orderRepository;

        public CreateOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<int> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var orderEntity = command.ToEntity();
            var genereatedOrder = await _orderRepository.AddAsync(orderEntity);
            return genereatedOrder.Id;
        }
    }
}
