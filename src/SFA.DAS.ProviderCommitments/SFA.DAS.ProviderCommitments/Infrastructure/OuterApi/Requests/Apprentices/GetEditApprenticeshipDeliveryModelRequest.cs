using System.Collections.Generic;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices
{
    public class GetEditApprenticeshipDeliveryModelRequest : IGetApiRequest
    {
        public long ProviderId { get; }
        public long ApprenticeshipId { get; set; }

        public GetEditApprenticeshipDeliveryModelRequest(long providerId, long apprenticeshipId)
        {
            ProviderId = providerId;
            ApprenticeshipId = apprenticeshipId;
        }

        public string GetUrl => $"provider/{ProviderId}/apprentices/{ApprenticeshipId}/edit/delivery-model";
    }

    public class GetEditApprenticeshipDeliveryModelResponse
    {
        public string LegalEntityName { get; set; }
        public DeliveryModel DeliveryModel { get; set; }
        public List<DeliveryModel> DeliveryModels { get; set; }
    }
}
