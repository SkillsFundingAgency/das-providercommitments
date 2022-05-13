namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class GetDraftApprenticeshipsRequest : IGetApiRequest
    {
        private readonly long _cohortId;
        public string GetUrl => $"draftApprenticeship/{_cohortId}";

        public GetDraftApprenticeshipsRequest(long cohortId)
        {
            _cohortId = cohortId;
        }
    }
}
