using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class ViewDraftApprenticeshipRequest
    {
        public DraftApprenticeshipRequest Request { get; set; }
        public GetCohortResponse Cohort { get; set; }
    }
}
