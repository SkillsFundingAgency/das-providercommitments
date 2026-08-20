using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate
{
    public class DraftApprenticeshipOverlapOptionViewModel : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string CohortReference { get; set; }
        public OverlapOptions? OverlapOptions { get; set; }
        public string DraftApprenticeshipHashedId { get; set; }
        public long? DraftApprenticeshipId { get; set; }
        public ApprenticeshipStatus Status { get; set; }
        public bool EnableStopRequestEmail { get; set; }
        public bool HasWithdrawnStatusCode { get; set; }
        public bool IsSameProvider { get; set; }
        public string ProviderName { get; set; }
    }

    public enum OverlapOptions
    {
        SendStopRequest,
        ContactTheEmployer,
        CompleteActionLater
    }
}