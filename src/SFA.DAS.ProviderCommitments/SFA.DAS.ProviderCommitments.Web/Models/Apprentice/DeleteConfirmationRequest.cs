using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class DeleteConfirmationRequest : IAuthorizationContextModel
    {        
        public long ProviderId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public string CohortReference { get; set; }
    }
}
