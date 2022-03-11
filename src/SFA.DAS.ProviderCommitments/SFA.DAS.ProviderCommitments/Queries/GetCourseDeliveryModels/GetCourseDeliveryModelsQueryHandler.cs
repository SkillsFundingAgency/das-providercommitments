using MediatR;
using SFA.DAS.ProviderCommitments.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Queries.GetCourseDeliveryModels
{
    public class GetCourseDeliveryModelsQueryHandler 
        : IRequestHandler<GetCourseDeliveryModelsQueryRequest, GetCourseDeliveryModelsQueryResponse>
    {
        private readonly ApprovalsOuterApiClient _client;

        public GetCourseDeliveryModelsQueryHandler(ApprovalsOuterApiClient client)
        {
            _client = client;
        }

        public async Task<GetCourseDeliveryModelsQueryResponse> Handle(
            GetCourseDeliveryModelsQueryRequest request,
            CancellationToken cancellationToken)
        {
            var models = await _client
                .GetCourseDeliveryModels(request.ProviderId, request.CourseId);
            
            return new GetCourseDeliveryModelsQueryResponse
            {
                DeliveryModels = models.DeliveryModels,
            };
        }
    }
}