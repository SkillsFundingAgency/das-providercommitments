using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class DraftApprenticeshipRequest : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public long CohortId { get; set; }

        public string CohortReference { get; set; }

        public long DraftApprenticeshipId { get; set; }

        public string DraftApprenticeshipHashedId { get; set; }
    }
}
