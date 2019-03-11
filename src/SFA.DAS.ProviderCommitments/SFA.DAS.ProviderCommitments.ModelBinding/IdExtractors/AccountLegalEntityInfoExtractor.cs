﻿using SFA.DAS.ProviderCommitments.HashingTemp;

namespace SFA.DAS.ProviderCommitments.ModelBinding.IdExtractors
{
    public class AccountLegalEntityInfoExtractor : HashedPropertyModelBinder
    {
        public AccountLegalEntityInfoExtractor(IHashingService publicAccountLegalEntityIdHashingService) : 
            base(publicAccountLegalEntityIdHashingService, RouteValueKeys.AccountLegalEntityId)
        {
            // just call base    
        }
    }
}