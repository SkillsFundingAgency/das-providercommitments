using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.ModelBinding;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

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
        public string Reference { get; set; }
        public string FirstName { get; set; }
        public Party Party { get; set; }
        public string Email { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public ViewEditBanners Banners { get; set; } = 0;
    }
}
