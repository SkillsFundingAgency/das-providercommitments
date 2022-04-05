using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class DraftApprenticeshipRequest : BaseDraftApprenticeshipRequest, IAuthorizationContextModel
    {
        public long CohortId { get; set; }
        public long DraftApprenticeshipId { get; set; }
    }

    public class BaseDraftApprenticeshipRequest
    {
        public long ProviderId { get; set; }
        public string CohortReference { get; set; }
        public string DraftApprenticeshipHashedId { get; set; }
    }
}
