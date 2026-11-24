using Ordering.Application.Abstractions;
using Ordering.Application.DTOs;

namespace Ordering.Application.Orders.GetOrders
{
    public record GetOrderListQuery(string userName) : IQuery<List<OrderDto>>;
}
