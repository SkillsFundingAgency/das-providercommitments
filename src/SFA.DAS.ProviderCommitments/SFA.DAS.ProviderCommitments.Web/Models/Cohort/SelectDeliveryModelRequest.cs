using System;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class SelectDeliveryModelRequest
    {
        public long ProviderId { get; set; }
        public Guid CacheKey { get; set; }
    }
}
