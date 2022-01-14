using Microsoft.AspNetCore.Http;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class FileUploadStartViewModel
    {
        public IFormFile Attachment { get; set; }
        public long ProviderId { get; set; }
    }
}