using System.Collections.Generic;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship
{
    public class GetEditDraftApprenticeshipSelectDeliveryModelRequest : IGetApiRequest
    {
        public string CourseCode { get; }
        public long ProviderId { get; }
        public long CohortId { get; set; }
        public long DraftApprenticeshipId { get; set; }

        public GetEditDraftApprenticeshipSelectDeliveryModelRequest(long providerId, long cohortId, long draftApprenticeshipId, string courseCode)
        {
            CourseCode = courseCode;
            ProviderId = providerId;
            CohortId = cohortId;
            DraftApprenticeshipId = draftApprenticeshipId;
        }

        public string GetUrl => $"provider/{ProviderId}/unapproved/{CohortId}/apprentices/{DraftApprenticeshipId}/edit/select-delivery-model?courseCode={CourseCode}";
    }

    public class GetEditDraftApprenticeshipSelectDeliveryModelResponse
    {
        public string EmployerName { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
        public List<DeliveryModel> DeliveryModels { get; set; }
        public bool HasUnavailableDeliveryModel { get; set; }
    }
}
