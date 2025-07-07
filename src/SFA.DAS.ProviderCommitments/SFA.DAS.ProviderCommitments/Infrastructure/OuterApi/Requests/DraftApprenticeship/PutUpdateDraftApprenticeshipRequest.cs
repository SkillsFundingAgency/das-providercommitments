namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship
{
    public class PutUpdateDraftApprenticeshipRequest(long cohortId, long apprenticeshipId) : IPutApiRequest
    {
        public object Data { get; set; }

        public string PutUrl => $"cohorts/{cohortId}/draft-apprenticeships/{apprenticeshipId}";
    }
}
