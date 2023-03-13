namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship
{
    public class PostUpdateDraftApprenticeshipRequest : IPostApiRequest
    {
        private readonly long _cohortId;
        private readonly long _apprenticeshipId;
        public object Data { get; set; }

        public PostUpdateDraftApprenticeshipRequest(long cohortId, long apprenticeshipId)
        {
            _cohortId = cohortId;
            _apprenticeshipId = apprenticeshipId;
        }
        public string PostUrl => $"cohorts/{_cohortId}/draft-apprenticeships/{_apprenticeshipId}";
    }
}
