using SFA.DAS.ProviderCommitments.Web.ModelBinding;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate
{
    public class DraftApprenticeshipOverlapOptionWithPendingRequest : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string CohortReference { get; set; }
        public string DraftApprenticeshipHashedId { get; set; }
        public long? DraftApprenticeshipId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public long? ApprenticeshipId { get; set; }
        public DateTime CreatedOn { get; set; }
        public ApprenticeshipStatus Status { get; set; }
        public bool EnableStopRequestEmail { get; set; }
    }
}
