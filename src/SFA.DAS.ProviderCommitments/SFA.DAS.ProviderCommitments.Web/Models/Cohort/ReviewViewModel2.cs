using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class ReviewViewModel2
    {
        public long ProviderId { get; set; }
        public IEnumerable<ReviewCohortSummaryViewModel2> Cohorts { get; set; }
    }
}
