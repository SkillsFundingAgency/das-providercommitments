namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices.ChangeEmployer
{
    public class GetInformRequest : IGetApiRequest
    {
        public long ProviderId { get; }
        public long ApprenticeshipId { get; set; }

        public GetInformRequest(long providerId, long apprenticeshipId)
        {
            ProviderId = providerId;
            ApprenticeshipId = apprenticeshipId;
        }

        public string GetUrl => $"provider/{ProviderId}/apprentices/{ApprenticeshipId}/change-employer/inform";
    }

    public class GetInformResponse
    {
        public string LegalEntityName { get; set; }
    }

}
