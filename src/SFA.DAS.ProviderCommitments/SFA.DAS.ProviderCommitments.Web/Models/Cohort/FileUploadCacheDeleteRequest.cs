using System;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class FileUploadCacheDeleteRequest
    {
        public long ProviderId { get; set; }
        public Guid CacheRequestId { get; set; }
        public FileUploadCacheDeleteRedirect? RedirectTo { get; set; }
    }

    public enum FileUploadCacheDeleteRedirect
    {
        UploadAgain,
        Home
    }
}