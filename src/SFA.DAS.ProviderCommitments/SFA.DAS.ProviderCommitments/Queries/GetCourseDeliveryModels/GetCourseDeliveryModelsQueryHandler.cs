using MediatR;
using SFA.DAS.ProviderCommitments.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Queries.GetCourseDeliveryModels
{
    public class GetCourseDeliveryModelsQueryHandler 
        : IRequestHandler<GetCourseDeliveryModelsQueryRequest, GetCourseDeliveryModelsQueryResponse>
    {
        private readonly CommitmentsOuterApiClient _client;

        public GetCourseDeliveryModelsQueryHandler(CommitmentsOuterApiClient client)
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
                Models = models,
            };
        }
    }
}