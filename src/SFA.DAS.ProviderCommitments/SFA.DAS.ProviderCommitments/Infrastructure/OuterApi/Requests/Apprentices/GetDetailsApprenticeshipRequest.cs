namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices
{
    public class GetDetailsApprenticeshipRequest : IGetApiRequest
    {
        public long ProviderId { get; }
        public long ApprenticeshipId { get; set; }

        public GetDetailsApprenticeshipRequest(long providerId, long apprenticeshipId)
        {
            ProviderId = providerId;
            ApprenticeshipId = apprenticeshipId;
        }

        public string GetUrl => $"provider/{ProviderId}/apprentices/{ApprenticeshipId}/details";
    }

    public class GetDetailsApprenticeshipResponse
    {
        public bool HasMultipleDeliveryModelOptions { get; set; }
    }

}
