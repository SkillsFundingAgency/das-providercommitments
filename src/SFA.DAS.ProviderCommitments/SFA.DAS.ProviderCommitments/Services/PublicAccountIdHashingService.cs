using SFA.DAS.ProviderCommitments.Configuration;

namespace SFA.DAS.ProviderCommitments.Services
{
    public class PublicAccountIdHashingService : HashingService.HashingService, IPublicAccountIdHashingService
    {
        public PublicAccountIdHashingService(PublicAccountIdHashingConfiguration configuration) : 
            base(configuration.Alphabet, configuration.Salt)
        {
            // just call base    
        }
    }
}