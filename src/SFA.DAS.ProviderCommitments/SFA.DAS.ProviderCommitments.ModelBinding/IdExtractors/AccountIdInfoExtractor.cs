using SFA.DAS.ProviderCommitments.HashingTemp;

namespace SFA.DAS.ProviderCommitments.ModelBinding.IdExtractors
{
    public class AccountIdInfoExtractor : HashedPropertyModelBinder
    {
        public AccountIdInfoExtractor(IHashingService publicAccountIdHashingService) : 
            base(publicAccountIdHashingService, RouteValueKeys.AccountId)
        {
            // just call base   
        }
    }
}