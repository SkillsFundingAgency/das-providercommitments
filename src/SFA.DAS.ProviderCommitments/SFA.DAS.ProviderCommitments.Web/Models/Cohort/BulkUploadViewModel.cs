using Microsoft.AspNetCore.Http;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class BulkUploadViewModel : IAuthorizationContextModel
    {
        public IFormFile Attachment { get; set; }
        public long ProviderId { get; set; }
        public string HashedCommitmentId { get; set; }
        public int ApprenticeshipCount { get; set; }
        public int ErrorCount { get; set; }
        public int WarningsCount { get; set; }
        public int RowCount { get; set; }
        public bool IsPaidByTransfer { get; set; }
        public long AccountId { get; set; }
        public long LegalEntityId { get; set; }
    }
}
