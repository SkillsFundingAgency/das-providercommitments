using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class ReviewViewModel
    {
        public long ProviderId { get; set; }
        public IEnumerable<ReviewCohortSummaryViewModel> Cohorts { get; set; }
    }
}
