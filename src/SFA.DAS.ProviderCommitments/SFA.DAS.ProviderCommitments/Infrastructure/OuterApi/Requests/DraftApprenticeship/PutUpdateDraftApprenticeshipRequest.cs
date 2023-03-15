namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship
{
    public class PutUpdateDraftApprenticeshipRequest : IPutApiRequest
    {
        private readonly long _cohortId;
        private readonly long _apprenticeshipId;
        public object Data { get; set; }

        public PutUpdateDraftApprenticeshipRequest(long cohortId, long apprenticeshipId)
        {
            _cohortId = cohortId;
            _apprenticeshipId = apprenticeshipId;
        }
        public string PutUrl => $"cohorts/{_cohortId}/draft-apprenticeships/{_apprenticeshipId}";
    }
}
