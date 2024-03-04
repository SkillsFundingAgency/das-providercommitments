namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class ReviewViewModel : SortViewModel
    {
        public long ProviderId { get; set; }
        public ApprenticeshipRequestsHeaderViewModel ApprenticeshipRequestsHeaderViewModel { get; set; }
        public IEnumerable<ReviewCohortSummaryViewModel> Cohorts { get; set; }
    }
}