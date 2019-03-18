using SFA.DAS.ProviderCommitments.Configuration;

namespace SFA.DAS.ProviderCommitments.Services
{
    public class PublicAccountLegalEntityIdHashingService : HashingService.HashingService, IPublicAccountLegalEntityIdHashingService
    {
        public PublicAccountLegalEntityIdHashingService(PublicAccountLegalEntityIdHashingConfiguration configuration) :
            base(configuration.Alphabet, configuration.Salt)
        {
            // just call base    
        }
    }
}