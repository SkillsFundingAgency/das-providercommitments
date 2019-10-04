using System.Threading.Tasks;
using SFA.DAS.Commitments.Shared.Models;

namespace SFA.DAS.Commitments.Shared.Interfaces
{
    interface IEmployerAgreementService
    {
        Task<bool> IsAgreementSigned(long accountId, long accountLegalEntityId, params AgreementFeature[] requiredFeatures);
    }
}
