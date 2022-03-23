using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class AddDraftApprenticeshipViewModel : DraftApprenticeshipViewModel, IAuthorizationContextModel
    {
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        
        public long AccountLegalEntityId { get; set; }
    }
}