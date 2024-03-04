using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate
{
    public class DraftApprenticeshipOverlapOptionRequest : IAuthorizationContextModel
    {
        public string DraftApprenticeshipHashedId { get; set; }
        public long? DraftApprenticeshipId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public long? ApprenticeshipId { get; set; }
        public string CohortReference { get; set; }
        public long ProviderId { get; set; }
    }
}
