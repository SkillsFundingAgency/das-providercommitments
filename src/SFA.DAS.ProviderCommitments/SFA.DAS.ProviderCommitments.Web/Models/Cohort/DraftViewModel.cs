namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class DraftViewModel : SortViewModel
    {
        public long ProviderId { get; set; }
        public ApprenticeshipRequestsHeaderViewModel ApprenticeshipRequestsHeaderViewModel { get; set; }
        public IEnumerable<DraftCohortSummaryViewModel> Cohorts { get; set; }
    }
}