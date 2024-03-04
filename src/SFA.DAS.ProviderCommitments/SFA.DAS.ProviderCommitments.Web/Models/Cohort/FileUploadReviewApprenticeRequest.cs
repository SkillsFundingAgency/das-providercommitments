namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class FileUploadReviewApprenticeRequest
    {
        public long ProviderId { get; set; }
        public Guid CacheRequestId { get; set; }
        public string CohortRef { get; set; }
        public string AgreementId { get; set; }
    }
}
