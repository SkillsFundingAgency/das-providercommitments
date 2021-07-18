using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class DatalockConfirmRestartRequest : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        
        public string ApprenticeshipHashedId { get; set; }

        public long ApprenticeshipId { get; set; }
    }
}
