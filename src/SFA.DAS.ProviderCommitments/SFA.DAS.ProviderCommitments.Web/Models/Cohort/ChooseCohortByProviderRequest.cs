using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class ChooseCohortByProviderRequest : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string SortField { get; set; } = nameof(ChooseCohortSummaryViewModel.CreatedOn);
        public bool ReverseSort { get; set; } = true;
        public bool UseLearnerData { get; set; }
    }
}
