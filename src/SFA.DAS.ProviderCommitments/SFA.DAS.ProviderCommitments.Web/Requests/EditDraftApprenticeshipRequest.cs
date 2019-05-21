using SFA.DAS.Authorization;

namespace SFA.DAS.ProviderCommitments.Web.Requests
{
    public class EditDraftApprenticeshipRequest : IAuthorizationContextModel
    {
        public long? CohortId { get; set; }

        public string CohortReference { get; set; }

        public long? DraftApprenticeshipId { get; set; }

        public string DraftApprenticeshipHashedId { get; set; }
    }
}
