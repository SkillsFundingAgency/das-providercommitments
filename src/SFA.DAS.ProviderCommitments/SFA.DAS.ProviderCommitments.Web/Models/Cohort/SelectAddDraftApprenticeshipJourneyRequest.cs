using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class SelectAddDraftApprenticeshipJourneyRequest : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
    }
}