using Basket.Application.Commands;
using Basket.Application.Mappers;
using Basket.Application.Responses;
using Basket.Core.Repositories;
using MediatR;

namespace Basket.Application.Handlers
{
    public class CreateShoppingCartCommandHandler : IRequestHandler<CreateShoppingCartCommand, ShoppingCartResponse>
    {
        private readonly IBasketRepository _basketRepsitory;

        public CreateShoppingCartCommandHandler(IBasketRepository IBasketRepository)
        {
            _basketRepsitory = IBasketRepository;
        }
        public async Task<ShoppingCartResponse> Handle(CreateShoppingCartCommand request, CancellationToken cancellationToken)
        {
            // Convert commmand to domain entity
            var shoppingCartEntity = request.ToEntity();
            // Save to redis
            var updatedCart = await _basketRepsitory.UpsertBasket(shoppingCartEntity);
            // Convert back to response
            return updatedCart.ToResponse();    
        }
    }
}
