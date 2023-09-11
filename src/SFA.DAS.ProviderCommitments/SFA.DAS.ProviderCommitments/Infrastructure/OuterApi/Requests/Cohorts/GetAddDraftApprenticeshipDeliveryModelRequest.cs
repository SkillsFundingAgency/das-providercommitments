using System.Collections.Generic;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts
{
    public class GetAddDraftApprenticeshipDeliveryModelRequest : IGetApiRequest
    {
        public string CourseCode { get; }
        public long ProviderId { get; }
        public long AccountLegalEntityId { get; set; }

        public GetAddDraftApprenticeshipDeliveryModelRequest(long providerId, long accountLegalEntityId, string courseCode)
        {
            CourseCode = courseCode;
            ProviderId = providerId;
            AccountLegalEntityId = accountLegalEntityId;
        }

        public string GetUrl => $"provider/{ProviderId}/unapproved/add/select-delivery-model?courseCode={CourseCode}&accountLegalEntityId={AccountLegalEntityId}";
    }

    public class GetAddDraftApprenticeshipDeliveryModelResponse
    {
        public string EmployerName { get; set; }
        public List<DeliveryModel> DeliveryModels { get; set; }
    }
}
