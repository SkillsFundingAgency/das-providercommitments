using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class WithEmployerViewModel : SortViewModel
    {
        public long ProviderId { get; set; }
        public ApprenticeshipRequestsHeaderViewModel ApprenticeshipRequestsHeaderViewModel { get; set; }
        public IEnumerable<WithEmployerSummaryViewModel> Cohorts { get; set; }
    }
}
