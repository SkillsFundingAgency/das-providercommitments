using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class WithTransferSenderViewModel
    {
        public long ProviderId { get; set; }
        public IEnumerable<WithTransferSenderCohortSummaryViewModel> Cohorts { get; set; }
    }
}
