using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class ChooseCohortViewModel
    {
        public long ProviderId { get; set; }
        public IEnumerable<ChooseCohortSummaryViewModel> Cohorts { get; set; }
    }
}