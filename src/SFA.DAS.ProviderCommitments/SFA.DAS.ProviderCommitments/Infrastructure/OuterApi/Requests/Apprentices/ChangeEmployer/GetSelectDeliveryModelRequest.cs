using System.Collections.Generic;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices.ChangeEmployer
{
    public class GetSelectDeliveryModelRequest : IGetApiRequest
    {
        public long ProviderId { get; }
        public long ApprenticeshipId { get; set; }
        public long AccountLegalEntityId { get; set; }

        public GetSelectDeliveryModelRequest(long providerId, long apprenticeshipId, long accountLegalEntityId)
        {
            ProviderId = providerId;
            ApprenticeshipId = apprenticeshipId;
            AccountLegalEntityId = accountLegalEntityId;
        }

        public string GetUrl => $"provider/{ProviderId}/apprentices/{ApprenticeshipId}/change-employer/select-delivery-model?accountLegalEntityId={AccountLegalEntityId}";
    }

    public class GetSelectDeliveryModelResponse
    {
        public string LegalEntityName { get; set; }
        public List<DeliveryModel> DeliveryModels { get; set; }
    }
}
