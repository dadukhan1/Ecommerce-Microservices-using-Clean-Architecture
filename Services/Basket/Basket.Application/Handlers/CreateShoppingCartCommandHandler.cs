using Basket.Application.Commands;
using Basket.Application.GrpcService;
using Basket.Application.Mappers;
using Basket.Application.Responses;
using Basket.Core.Repositories;
using MediatR;

namespace Basket.Application.Handlers
{
    public class CreateShoppingCartCommandHandler : IRequestHandler<CreateShoppingCartCommand, ShoppingCartResponse>
    {
        private readonly IBasketRepository _basketRepsitory;
        private readonly DiscountGrpcService _discountGrpcService;

        public CreateShoppingCartCommandHandler(IBasketRepository IBasketRepository, DiscountGrpcService discountGrpcService)
        {
            _basketRepsitory = IBasketRepository;
            _discountGrpcService = discountGrpcService;
        }
        public async Task<ShoppingCartResponse> Handle(CreateShoppingCartCommand request, CancellationToken cancellationToken)
        {
            // Apply discounts GRPC call
            foreach(var item in request.Items)
            {
                var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }
            // Convert commmand to domain entity
            var shoppingCartEntity = request.ToEntity();
            // Save to redis
            var updatedCart = await _basketRepsitory.UpsertBasket(shoppingCartEntity);
            // Convert back to response
            return updatedCart.ToResponse();    
        }
    }
}
