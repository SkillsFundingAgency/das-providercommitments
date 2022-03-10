using MediatR;

namespace SFA.DAS.ProviderCommitments.Queries.GetCourseDeliveryModels
{
    public class GetCourseDeliveryModelsQueryRequest : IRequest<GetCourseDeliveryModelsQueryResponse>
    {
        public long ProviderId { get; set; }
        public string CourseId { get; set; }
    }
}