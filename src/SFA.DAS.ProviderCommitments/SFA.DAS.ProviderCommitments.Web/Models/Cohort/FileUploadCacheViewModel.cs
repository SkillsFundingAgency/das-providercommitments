using System;
using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public partial class FileUploadCacheViewModel
    {
        public FileUploadCacheViewModel()
        {
            EmployerDetails = new List<FileUploadCacheEmployerDetails>();
        }
        public Guid CacheRequestId { get; set; }
        public long ProviderId { get; set; }
        public FileUploadCacheOption? SelectedOption { get; set; }
        public List<FileUploadCacheEmployerDetails> EmployerDetails { get; set; }
    }

    public enum FileUploadCacheOption
    {
        ApproveAndSend,
        SaveButDontSend,
        UploadAmendedFile
    }
}
