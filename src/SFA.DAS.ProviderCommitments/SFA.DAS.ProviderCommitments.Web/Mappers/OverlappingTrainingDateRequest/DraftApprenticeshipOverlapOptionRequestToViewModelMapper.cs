using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.OverlappingTrainingDateRequest
{
    public class DraftApprenticeshipOverlapOptionRequestToViewModelMapper : IMapper<DraftApprenticeshipOverlapOptionRequest, DraftApprenticeshipOverlapOptionViewModel>
    {
        private IFeatureTogglesService<ProviderFeatureToggle> _featureTogglesService;
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public DraftApprenticeshipOverlapOptionRequestToViewModelMapper(IFeatureTogglesService<ProviderFeatureToggle> featureTogglesService, 
            ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _featureTogglesService = featureTogglesService;
        }

        public async Task<DraftApprenticeshipOverlapOptionViewModel> Map(DraftApprenticeshipOverlapOptionRequest request)
        {
            var apprenticeshipDetails = _commitmentsApiClient.GetApprenticeship(request.ApprenticeshipId.Value).Result;

            var featureToggleEnabled = _featureTogglesService.GetFeatureToggle(ProviderFeature.OverlappingTrainingDateWithoutPrefix).IsEnabled;
            var vm = new DraftApprenticeshipOverlapOptionViewModel
            {
                DraftApprenticeshipHashedId = request.DraftApprenticeshipHashedId,
                OverlappingTrainingDateRequestToggleEnabled = featureToggleEnabled,
                Status = apprenticeshipDetails.Status,
                EnableStopRequestEmail = featureToggleEnabled && (apprenticeshipDetails.Status == CommitmentsV2.Types.ApprenticeshipStatus.Live
                || apprenticeshipDetails.Status == CommitmentsV2.Types.ApprenticeshipStatus.WaitingToStart
                || apprenticeshipDetails.Status == CommitmentsV2.Types.ApprenticeshipStatus.Paused
                || apprenticeshipDetails.Status == CommitmentsV2.Types.ApprenticeshipStatus.Completed
                || apprenticeshipDetails.Status == CommitmentsV2.Types.ApprenticeshipStatus.Stopped)
            };

            return vm;
        }
    }
}
