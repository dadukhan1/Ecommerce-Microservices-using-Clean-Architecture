
using Basket.Grpc.Protos;

namespace Basket.Application.GrpcService
{
    public class DiscountGrpcService
    {
        private readonly DiscountProtoServices.DiscountProtoServicesClient _discountProtoServiceClient;

        public DiscountGrpcService(DiscountProtoServices.DiscountProtoServicesClient discountProtoServiceClient)
        {
            _discountProtoServiceClient = discountProtoServiceClient;
        }
        public async Task<CouponModel> GetDiscount(string ProductName)
        {
            var discountRequest = new GetDiscountRequest { ProductName = ProductName };
            return await _discountProtoServiceClient.GetDiscountAsync(discountRequest);
        }
    }
}
