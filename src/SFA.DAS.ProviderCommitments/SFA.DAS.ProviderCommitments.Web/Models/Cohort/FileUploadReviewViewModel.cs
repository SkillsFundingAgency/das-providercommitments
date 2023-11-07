namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public partial class FileUploadReviewViewModel
    {
        public FileUploadReviewViewModel()
        {
            EmployerDetails = new List<FileUploadReviewEmployerDetails>();
        }
        public Guid CacheRequestId { get; set; }
        public long ProviderId { get; set; }
        public long? FileUploadLogId { get; set; }
        public bool CanApprove { get; set; }
        public FileUploadReviewOption? SelectedOption { get; set; }
        public List<FileUploadReviewEmployerDetails> EmployerDetails { get; set; }
    }

    public enum FileUploadReviewOption
    {
        ApproveAndSend,
        SaveButDontSend,
        UploadAmendedFile
    }
}
