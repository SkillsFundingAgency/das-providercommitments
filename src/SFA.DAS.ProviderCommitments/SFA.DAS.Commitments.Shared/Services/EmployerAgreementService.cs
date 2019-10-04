using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.Commitments.Shared.Models;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Encoding;

namespace SFA.DAS.Commitments.Shared.Services
{
    public class EmployerAgreementService : IEmployerAgreementService
    {
        private readonly IAccountApiClient _accountApiClient;
        private readonly IEncodingService _encodingService;

        private readonly Dictionary<AgreementFeature, int> _agreementUnlocks;

        public EmployerAgreementService(IAccountApiClient accountApiClient, IEncodingService encodingService)
        {
            _accountApiClient = accountApiClient;
            _encodingService = encodingService;

            //This dictionary indicates which features are unlocked by agreement versions
            _agreementUnlocks = new Dictionary<AgreementFeature, int> { { AgreementFeature.Transfers, 2 } };
        }


        public async Task<bool> IsAgreementSigned(long accountId, long accountLegalEntityId, params AgreementFeature[] requiredFeatures)
        {
            var hashedAccountId = _encodingService.Encode(accountId, EncodingType.AccountId);
            var legalEntity = await _accountApiClient.GetLegalEntity(hashedAccountId, accountLegalEntityId);

            //Determine the latest signed agreement version
            var latestSignedVersion = legalEntity.Agreements
                .Where(x => x.Status == EmployerAgreementStatus.Signed)
                .Max(y => y.TemplateVersionNumber);

            //No agreement signed
            if (latestSignedVersion == 0)
            {
                return false;
            }

            //No features beyond basic
            if (requiredFeatures.Length == 0)
            {
                return true;
            }

            //Evaluate requirements
            foreach (var requiredFeature in requiredFeatures)
            {
                if (latestSignedVersion < _agreementUnlocks[requiredFeature])
                {
                    return false; //A feature is required that has not been unlocked
                }
            }

            return true; //All requirements met
        }
    }
}
