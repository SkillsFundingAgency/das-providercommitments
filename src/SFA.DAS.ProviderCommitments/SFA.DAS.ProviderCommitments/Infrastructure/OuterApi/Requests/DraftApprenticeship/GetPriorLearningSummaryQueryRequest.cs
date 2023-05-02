using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest
{
    public class GetPriorLearningSummaryQueryRequest : IGetApiRequest
    {
        public long CohortId { get; set; }
        public long DraftApprenticeshipId { get; set; }

        public GetPriorLearningSummaryQueryRequest(long cohortId, long draftApprenticeshipId)
        {
            CohortId = cohortId;
            DraftApprenticeshipId = draftApprenticeshipId;
        }
        public string GetUrl => $"cohorts/{CohortId}/draft-apprenticeships/{DraftApprenticeshipId}/prior-learning-summary";
    }
}
