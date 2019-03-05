using SFA.DAS.ProviderCommitments.Services;

namespace SFA.DAS.ProviderCommitments.Web.RouteValues
{
    public class AccountLegalEntityInfoExtractor : HashedPropertyModelBinder
    {
        public AccountLegalEntityInfoExtractor(IPublicAccountLegalEntityIdHashingService hashingService): 
            base(hashingService, RouteValues.AccountLegalEntityPublicHashedId)
        {
            // just call base    
        }
    }
}