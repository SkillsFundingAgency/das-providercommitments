namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts
{
    public class GetHasDeclaredStandardsRequest : IGetApiRequest
    {
        public long ProviderId { get; }

        public GetHasDeclaredStandardsRequest(long providerId)
        {
            ProviderId = providerId;
        }

        public string GetUrl => $"provider/{ProviderId}/unapproved/add/confirm-employer";
    }

    public class GetHasDeclaredStandardsResponse
    {
        public bool HasNoDeclaredStandards { get; set; }
    }
}
