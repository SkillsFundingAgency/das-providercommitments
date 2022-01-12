using System;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class FileUploadCacheRequest
    {
        public long ProviderId { get; set; }
        public Guid CacheRequestId { get; set; }
    }
}