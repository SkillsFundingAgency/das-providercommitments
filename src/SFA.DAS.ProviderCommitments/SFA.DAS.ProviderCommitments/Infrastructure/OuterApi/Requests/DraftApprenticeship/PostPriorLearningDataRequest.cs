namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship
{
    public class PostPriorLearningDataRequest : IPostApiRequest
    {
        private readonly long _cohortId;
        private readonly long _draftApprenticeshipId;
        public object Data { get; set; }

        public PostPriorLearningDataRequest(long cohortId, long draftApprenticeshipId)
        {
            _cohortId = cohortId;
            _draftApprenticeshipId = draftApprenticeshipId;
        }
        public string PostUrl => $"cohorts/{_cohortId}/draft-apprenticeships/{_draftApprenticeshipId}";
    }
}
