using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.ProviderCommitments.Web.LocalDevRegistry;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class BulkUploadSummaryViewModel : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public BulkUploadResponse BulkUploadResponse { get; set; }
        public CohortAction CohortAction { get; set; }
    }
}
