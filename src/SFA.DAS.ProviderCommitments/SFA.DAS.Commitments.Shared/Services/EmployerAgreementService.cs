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

        public async Task<bool> IsAgreementSigned(long accountId, long accountLegalEntityId,  params AgreementFeature[] requiredFeatures)
        {
            var hashedAccountId = _encodingService.Encode(accountId, EncodingType.AccountId);
            var legalEntity = await _accountApiClient.GetLegalEntity(hashedAccountId, accountLegalEntityId);

            var signedAgreements = legalEntity.Agreements
                .Where(x => x.Status == EmployerAgreementStatus.Signed).ToList();

            if (!signedAgreements.Any())
            {
                return false;
            }

            //No extended features required
            if (requiredFeatures.Length == 0)
            {
                return true;
            }

            var latestSignedVersion = signedAgreements.Max(x => x.TemplateVersionNumber);

            // If any required feature exceeds this agreement version return false 
            if (requiredFeatures.Any(f => latestSignedVersion < _agreementUnlocks[f]))
            {
                return false;
            }

            return true; //All requirements met
        }
    }
}
