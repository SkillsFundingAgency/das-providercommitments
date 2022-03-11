using MediatR;
using SFA.DAS.ProviderCommitments.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Queries.GetProviderCourseDeliveryModels
{
    public class GetProviderCourseDeliveryModelsQueryHandler
        : IRequestHandler<
            GetProviderCourseDeliveryModelsQueryRequest,
            GetProviderCourseDeliveryModelsQueryResponse>
    {
        private readonly ApprovalsOuterApiClient _client;

        public GetProviderCourseDeliveryModelsQueryHandler(ApprovalsOuterApiClient client)
        {
            _client = client;
        }

        public async Task<GetProviderCourseDeliveryModelsQueryResponse> Handle(
            GetProviderCourseDeliveryModelsQueryRequest request,
            CancellationToken cancellationToken)
        {
            var models = await _client
                .GetProviderCourseDeliveryModels(request.ProviderId, request.CourseId);

            return new GetProviderCourseDeliveryModelsQueryResponse
            {
                DeliveryModels = models.DeliveryModels,
            };
        }
    }
}