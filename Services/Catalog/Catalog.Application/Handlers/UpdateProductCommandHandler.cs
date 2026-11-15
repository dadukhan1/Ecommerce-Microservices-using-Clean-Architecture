using Catalog.Application.Commands;
using Catalog.Application.Mappers;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;

        public UpdateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var existing = await _productRepository.GetProduct(request.Id);
            if(existing == null)
            {
                throw new KeyNotFoundException($"Product with Id {request.Id} not found");
            }
            // step 1 : fetch brand and type 
            var brand = await _productRepository.GetBrandByIdAsync(request.BrandId);
            var type = await _productRepository.GetTypeByIdAsync(request.TypeId);
            if (brand == null || type == null)
            {
                throw new ApplicationException("Invalid Brand or Type Specified");
            }

            // step 2 : Mapper role
            var updatedProduct = request.ToUpdateEntity(existing, brand, type);
            // step 3 : save the record
            return await _productRepository.UpdateProduct(updatedProduct);
        }
    }
}
