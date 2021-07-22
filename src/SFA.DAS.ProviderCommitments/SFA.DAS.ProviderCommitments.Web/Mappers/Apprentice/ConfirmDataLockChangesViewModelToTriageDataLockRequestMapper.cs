using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class ConfirmDataLockChangesViewModelToTriageDataLockRequestMapper : IMapper<ConfirmDataLockChangesViewModel, TriageDataLocksRequest>
    {
        private readonly IAuthenticationService _authenticationService;

        public ConfirmDataLockChangesViewModelToTriageDataLockRequestMapper(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<TriageDataLocksRequest> Map(ConfirmDataLockChangesViewModel source)
        {
            return await Task.FromResult(new TriageDataLocksRequest
            {
                ApprenticeshipId = source.ApprenticeshipId,
                TriageStatus = CommitmentsV2.Types.TriageStatus.Change,
                UserInfo = _authenticationService.UserInfo
            });
        }
    }
}
