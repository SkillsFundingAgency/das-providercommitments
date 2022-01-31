using System;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class FileUploadReviewDeleteRequest
    {
        public long ProviderId { get; set; }
        public Guid CacheRequestId { get; set; }
        public FileUploadReviewDeleteRedirect? RedirectTo { get; set; }
    }

    public enum FileUploadReviewDeleteRedirect
    {
        UploadAgain,
        Home,
        SuccessDiscardFile
    }
}