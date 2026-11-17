using Basket.Application.Commands;
using Basket.Core.Repositories;
using MediatR;

namespace Basket.Application.Handlers
{
    public class DeleteBasketByUserNameCommandHandler : IRequestHandler<DeleteBasketByUserNameCommand, Unit>
    {
        private readonly IBasketRepository _basketRepsitory;

        public DeleteBasketByUserNameCommandHandler(IBasketRepository basketRepsitory)
        {
            _basketRepsitory = basketRepsitory;
        }
        public async Task<Unit> Handle(DeleteBasketByUserNameCommand request, CancellationToken cancellationToken)
        {
            await _basketRepsitory.DeleteBasket(request.userName);
            return Unit.Value;
        }
    }
}
