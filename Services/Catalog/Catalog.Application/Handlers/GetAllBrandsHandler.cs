using Catalog.Application.Mappers;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers
{
    public class GetAllBrandsHandler : IRequestHandler<GetAllBrandsQuery, IList<BrandResponse>>
    {
        private IBrandRepository _brandRepository;

        public GetAllBrandsHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }
        async Task<IList<BrandResponse>> IRequestHandler<GetAllBrandsQuery, IList<BrandResponse>>.Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
        {
            var brandList = await _brandRepository.GetAllBrands();
            return brandList.ToResponseList();
        }
    }
}
