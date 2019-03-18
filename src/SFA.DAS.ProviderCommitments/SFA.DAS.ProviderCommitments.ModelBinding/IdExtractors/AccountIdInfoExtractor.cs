
using SFA.DAS.HashingService;

namespace SFA.DAS.ProviderCommitments.ModelBinding.IdExtractors
{
    /// <summary>
    ///     Extracts the hashed account id from the request and makes it available to the model binder.
    /// </summary>
    public class AccountIdInfoExtractor : HashedPropertyModelBinder
    {
        public AccountIdInfoExtractor(IHashingService publicAccountIdHashingService) : 
            base(publicAccountIdHashingService, RouteValueKeys.AccountId)
        {
            // just call base   
        }
    }
}