namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship
{
    public class PostAddDraftApprenticeshipRequest(long cohortId) : IPostApiRequest
    {
        public object Data { get; set; }

        public string PostUrl => $"cohorts/{cohortId}/draft-apprenticeships";
    }
}
