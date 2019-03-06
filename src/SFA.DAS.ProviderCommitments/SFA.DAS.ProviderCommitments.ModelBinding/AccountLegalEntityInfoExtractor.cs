using SFA.DAS.ProviderCommitments.Services;
using SFA.DAS.ProviderCommitments.Services.Temp;

namespace SFA.DAS.ProviderCommitments.Web.RouteValues
{
    public class AccountLegalEntityInfoExtractor : HashedPropertyModelBinder
    {
        public AccountLegalEntityInfoExtractor(IHashingService publicAccountLegalEntityIdHashingService) : 
            base(publicAccountLegalEntityIdHashingService, RouteValues.AccountLegalEntityPublicHashedId)
        {
            // just call base    
        }
    }
}