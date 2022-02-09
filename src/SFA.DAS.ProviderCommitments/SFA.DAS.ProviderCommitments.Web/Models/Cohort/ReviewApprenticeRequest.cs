using System;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class ReviewApprenticeRequest
    {
        public long ProviderId { get; set; }
        public Guid CacheRequestId { get; set; }
        public string CohortRef { get; set; }
    }
}
