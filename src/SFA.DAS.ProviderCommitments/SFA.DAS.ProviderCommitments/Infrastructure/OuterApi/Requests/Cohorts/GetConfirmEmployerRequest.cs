namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts
{
    public class GetConfirmEmployerRequest : IGetApiRequest
    {
        public long ProviderId { get; }

        public GetConfirmEmployerRequest(long providerId)
        {
            ProviderId = providerId;
        }

        public string GetUrl => $"provider/{ProviderId}/unapproved/add/confirm-employer";
    }

    public class GetConfirmEmployerResponse
    {
        public bool HasNoDeclaredStandards { get; set; }
    }
}
