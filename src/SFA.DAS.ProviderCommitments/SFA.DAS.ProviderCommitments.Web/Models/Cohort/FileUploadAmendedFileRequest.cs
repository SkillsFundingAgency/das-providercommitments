using System;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class FileUploadAmendedFileRequest
    {
        public long ProviderId { get; set; }
        public Guid CacheRequestId { get; set; }
    }
}
