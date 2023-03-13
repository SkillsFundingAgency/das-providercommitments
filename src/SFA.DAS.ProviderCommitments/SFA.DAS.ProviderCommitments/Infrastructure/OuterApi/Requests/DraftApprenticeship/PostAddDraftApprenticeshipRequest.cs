namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship
{
    public class PostAddDraftApprenticeshipRequest : IPostApiRequest
    {
        private readonly long _cohortId;
        public object Data { get; set; }

        public PostAddDraftApprenticeshipRequest(long cohortId)
        {
            _cohortId = cohortId;
        }
        public string PostUrl => $"cohorts/{_cohortId}/draft-apprenticeships";
    }
}
