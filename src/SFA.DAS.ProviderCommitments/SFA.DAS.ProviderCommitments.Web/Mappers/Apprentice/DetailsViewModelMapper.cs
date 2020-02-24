using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.ProviderCommitments.Features;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class DetailsViewModelMapper : IMapper<DetailsRequest, DetailsViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentApiClient;
        private readonly IEncodingService _encodingService;
        private readonly IFeatureTogglesService<ProviderFeatureToggle> _featureTogglesService;

        public DetailsViewModelMapper(ICommitmentsApiClient commitmentApiClient, IEncodingService encodingService,
            IFeatureTogglesService<ProviderFeatureToggle> featureTogglesService)
        {
            _commitmentApiClient = commitmentApiClient;
            _encodingService = encodingService;
            _featureTogglesService = featureTogglesService;
        }

        public async Task<DetailsViewModel> Map(DetailsRequest source)
        {
            var detailsResponseTask = _commitmentApiClient.GetApprenticeship(source.ApprenticeshipId);
            var priceEpisodes = await _commitmentApiClient.GetPriceEpisodes(source.ApprenticeshipId);
            var detailsResponse = await detailsResponseTask;

            var changeOfEmployerEnabled = _featureTogglesService.GetFeatureToggle(nameof(ProviderFeature.ChangeOfEmployer))?.IsEnabled ?? false;

            return new DetailsViewModel
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                ApprenticeName = $"{detailsResponse.FirstName} {detailsResponse.LastName}",
                Employer = detailsResponse.EmployerName,
                Reference = _encodingService.Encode(detailsResponse.CohortId, EncodingType.CohortReference),
                Status = detailsResponse.Status,
                StopDate = detailsResponse.StopDate,
                AgreementId = _encodingService.Encode(detailsResponse.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId),
                DateOfBirth = detailsResponse.DateOfBirth,
                Uln = detailsResponse.Uln,
                CourseName = detailsResponse.CourseName,
                StartDate = detailsResponse.StartDate,
                EndDate = detailsResponse.EndDate,
                ProviderRef = detailsResponse.Reference,
                Cost = priceEpisodes.PriceEpisodes.GetPrice(),
                ChangeOfEmployerEnabled = changeOfEmployerEnabled
            };
        }
    }
}