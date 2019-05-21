using SFA.DAS.Authorization;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class AddDraftApprenticeshipViewModel : DraftApprenticeshipViewModel, IAuthorizationContextModel
    {
        public AddDraftApprenticeshipViewModel() : base()
        {
        }

        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        
        public long? AccountLegalEntityId { get; set; }

    }
}