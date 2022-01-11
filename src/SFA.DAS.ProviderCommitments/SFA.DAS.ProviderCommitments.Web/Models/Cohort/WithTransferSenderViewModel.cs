using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class WithTransferSenderViewModel : SortViewModel
    {
        public long ProviderId { get; set; }
        public ApprenticeshipRequestsHeaderViewModel ApprenticeshipRequestsHeaderViewModel { get; set; }
        public IEnumerable<WithTransferSenderCohortSummaryViewModel> Cohorts { get; set; }
    }
}
