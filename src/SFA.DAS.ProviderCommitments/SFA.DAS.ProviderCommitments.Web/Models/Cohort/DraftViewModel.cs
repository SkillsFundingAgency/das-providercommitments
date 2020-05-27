using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class DraftViewModel
    {
        public long ProviderId { get; set; }
        public IEnumerable<DraftCohortSummaryViewModel> Cohorts { get; set; }
    }
}