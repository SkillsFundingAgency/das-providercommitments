using SFA.DAS.HashingService;

namespace SFA.DAS.ProviderCommitments.ModelBinding.IdExtractors
{
    /// <summary>
    ///     Extracts the hashed account legal entity id from the request and makes it available to the model binder.
    /// </summary>
    public class AccountLegalEntityInfoExtractor : HashedPropertyModelBinder
    {
        public AccountLegalEntityInfoExtractor(IHashingService publicAccountLegalEntityIdHashingService) : 
            base(publicAccountLegalEntityIdHashingService, RouteValueKeys.AccountLegalEntityId)
        {
            // just call base    
        }
    }
}