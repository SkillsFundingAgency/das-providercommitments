using System;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class FileDiscardViewModel
    {
        public long ProviderId { get; set; }
        public Guid CacheRequestId { get; set; }
        public bool? FileDiscardConfirmed { get; set; }
    }
}
