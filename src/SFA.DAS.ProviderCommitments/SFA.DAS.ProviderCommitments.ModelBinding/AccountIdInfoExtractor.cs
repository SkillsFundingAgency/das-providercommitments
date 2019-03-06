using SFA.DAS.ProviderCommitments.Services;
using SFA.DAS.ProviderCommitments.Services.Temp;

namespace SFA.DAS.ProviderCommitments.Web.RouteValues
{
    public class AccountIdInfoExtractor : HashedPropertyModelBinder
    {
        public AccountIdInfoExtractor(IHashingService publicAccountIdHashingService) : 
            base(publicAccountIdHashingService, RouteValues.AccountId)
        {
            // just call base   
        }
    }
}