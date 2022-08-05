using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices.ChangeEmployer
{
    public class GetConfirmEmployerRequest : IGetApiRequest
    {
        public long ProviderId { get; }
        public long ApprenticeshipId { get; set; }
        public long AccountLegalEntityId { get; set; }

        public GetConfirmEmployerRequest(long providerId, long apprenticeshipId, long accountLegalEntityId)
        {
            ProviderId = providerId;
            ApprenticeshipId = apprenticeshipId;
            AccountLegalEntityId = accountLegalEntityId;
        }

        public string GetUrl => $"provider/{ProviderId}/apprentices/{ApprenticeshipId}/change-employer/confirm-employer?accountLegalEntityId={AccountLegalEntityId}";
    }

    public class GetConfirmEmployerResponse
    {
        public string LegalEntityName { get; set; }
        public string AccountName { get; set; }
        public string AccountLegalEntityName { get; set; }
        public bool IsFlexiJobAgency { get; set; }
        public DeliveryModel DeliveryModel { get; set; }
    }
}
