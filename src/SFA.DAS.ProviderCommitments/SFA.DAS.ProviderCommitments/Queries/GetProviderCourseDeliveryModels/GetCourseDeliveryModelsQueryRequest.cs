using MediatR;

namespace SFA.DAS.ProviderCommitments.Queries.GetProviderCourseDeliveryModels
{
    public class GetProviderCourseDeliveryModelsQueryRequest : IRequest<GetProviderCourseDeliveryModelsQueryResponse>
    {
        public long ProviderId { get; set; }
        public string CourseId { get; set; }
    }
}