using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.HashingTemp;

namespace SFA.DAS.ProviderCommitments.Services
{
    public class PublicAccountIdHashingService : HashingService, IPublicAccountIdHashingService
    {
        public PublicAccountIdHashingService(PublicAccountIdHashingConfiguration configuration) : 
            base(configuration.Alphabet, configuration.Salt)
        {
            // just call base    
        }
    }
}