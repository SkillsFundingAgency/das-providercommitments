using SFA.DAS.ProviderCommitments.Services;

namespace SFA.DAS.ProviderCommitments.Web.RouteValues
{
    public class AccountIdInfoExtractor : HashedPropertyModelBinder
    {
        public AccountIdInfoExtractor(IPublicAccountIdHashingService hashingService) : 
            base(hashingService, RouteValues.AccountId)
        {
            // just call base   
        }
    }
}