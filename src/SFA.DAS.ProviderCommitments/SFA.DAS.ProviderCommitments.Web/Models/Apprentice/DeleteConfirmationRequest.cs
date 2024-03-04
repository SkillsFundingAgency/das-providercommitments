using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class DeleteConfirmationRequest : IAuthorizationContextModel
    {        
        public long ProviderId { get; set; }
        public string DraftApprenticeshipHashedId { get; set; }
        public string CohortReference { get; set; }
    }
}
