using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Google.Rpc;
using Grpc.Core;
using GoogleStatus = Google.Rpc.Status;
using GrpcStatus = Grpc.Core.Status;

namespace Discount.Application.Extensions
{
    public static class GRPCErrorHelper
    {
        public static RpcException CreateValidationException(Dictionary<string, string> fieldErrors)
        {
            var fieldViolations = new List<BadRequest.Types.FieldViolation>();
            foreach (var error in fieldErrors)
            {
                fieldViolations.Add(new BadRequest.Types.FieldViolation
                {
                    Field = error.Key,
                    Description = error.Value
                });
            };
            // Now add bad request 
            var badrequest = new BadRequest();
            badrequest.FieldViolations.AddRange(fieldViolations);

            var status = new GoogleStatus
            {
                Code = (int)StatusCode.InvalidArgument,
                Message = "Validation Failed",
                Details = { Any.Pack(badrequest) }
            };

            var trailers = new Metadata
            {
                {"grpc-status-details-bin", status.ToByteArray() }
            };

            return new RpcException(new GrpcStatus(StatusCode.InvalidArgument, "Validation Errors"), trailers);
        }
    }
}
