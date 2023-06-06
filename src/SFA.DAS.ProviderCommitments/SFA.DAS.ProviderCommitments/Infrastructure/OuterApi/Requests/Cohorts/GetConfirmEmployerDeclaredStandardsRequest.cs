namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts
{
    public class GetConfirmEmployerDeclaredStandardsRequest : IGetApiRequest
    {
        public long ProviderId { get; }

        public GetConfirmEmployerDeclaredStandardsRequest(long providerId)
        {
            ProviderId = providerId;
        }

        public string GetUrl => $"provider/{ProviderId}/unapproved/add/confirm-employer";
    }

    public class GetConfirmEmployerDeclaredStandardsResponse
    {
        public bool HasNoDeclaredStandards { get; set; }
    }
}
