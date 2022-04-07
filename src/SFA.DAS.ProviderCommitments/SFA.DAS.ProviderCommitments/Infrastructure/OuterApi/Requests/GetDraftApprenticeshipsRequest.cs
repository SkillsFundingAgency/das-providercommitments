namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class GetDraftApprenticeshipsRequest : IGetApiRequest
    {
        private long _cohortId { get; }
        public string GetUrl => $"draftApprenticeship/{_cohortId}";

        public GetDraftApprenticeshipsRequest(long cohortId)
        {
            _cohortId = cohortId;
        }
    }
}
