using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class EditDraftApprenticeshipRequest
    {
        public DraftApprenticeshipRequest Request { get; set; }
        public GetCohortResponse Cohort { get; set; }
        public string LearnerDataSyncKey { get; set; }
    }
}
