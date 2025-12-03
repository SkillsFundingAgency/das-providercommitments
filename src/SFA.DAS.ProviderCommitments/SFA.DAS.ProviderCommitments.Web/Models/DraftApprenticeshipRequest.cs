using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class DraftApprenticeshipRequest : BaseDraftApprenticeshipRequest, IAuthorizationContextModel
    {
        public long CohortId { get; set; }
        public long DraftApprenticeshipId { get; set; }
        public string LearnerDataSyncKey { get; set; }
    }

    public class BaseDraftApprenticeshipRequest
    {
        public Guid CacheKey { get; set; }
        public long ProviderId { get; set; }
        public string CohortReference { get; set; }
        public string DraftApprenticeshipHashedId { get; set; }
    }
}
