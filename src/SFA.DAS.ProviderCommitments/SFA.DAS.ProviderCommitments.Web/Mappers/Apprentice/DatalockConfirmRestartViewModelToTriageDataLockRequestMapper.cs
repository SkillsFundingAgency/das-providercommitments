using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class DatalockConfirmRestartViewModelToTriageDataLockRequestMapper : IMapper<DatalockConfirmRestartViewModel, TriageDataLocksRequest>
    {
        private readonly IAuthenticationService _authenticationService;

        public DatalockConfirmRestartViewModelToTriageDataLockRequestMapper(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<TriageDataLocksRequest> Map(DatalockConfirmRestartViewModel source)
        {
            return await Task.FromResult(new TriageDataLocksRequest
            {
                ApprenticeshipId = source.ApprenticeshipId,
                TriageStatus = CommitmentsV2.Types.TriageStatus.Restart,
                UserInfo = _authenticationService.UserInfo
            });
        }
    }
}
