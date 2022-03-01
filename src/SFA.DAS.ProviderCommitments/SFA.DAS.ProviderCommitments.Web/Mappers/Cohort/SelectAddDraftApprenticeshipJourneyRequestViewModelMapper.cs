using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.PAS.Account.Api.ClientV2;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types.Dtos;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class SelectAddDraftApprenticeshipJourneyRequestViewModelMapper : IMapper<SelectAddDraftApprenticeshipJourneyRequest, SelectAddDraftApprenticeshipJourneyViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IProviderRelationshipsApiClient _providerRelationshipsApiClient;
        private readonly IFeatureTogglesService<ProviderFeatureToggle> _featureTogglesService;

        public SelectAddDraftApprenticeshipJourneyRequestViewModelMapper(
            ICommitmentsApiClient commitmentApiClient, 
            IProviderRelationshipsApiClient providerRelationshipsApiClient,
              IFeatureTogglesService<ProviderFeatureToggle> featureTogglesService)
        {
            _providerRelationshipsApiClient = providerRelationshipsApiClient;
            _commitmentsApiClient = commitmentApiClient;
            _featureTogglesService = featureTogglesService;
        }

        public async Task<SelectAddDraftApprenticeshipJourneyViewModel> Map(SelectAddDraftApprenticeshipJourneyRequest source)
        {
            var getCohortsTask = _commitmentsApiClient.GetCohorts(new GetCohortsRequest { ProviderId = source.ProviderId });
            var hasRelationshipTask = _providerRelationshipsApiClient.HasRelationshipWithPermission(new HasRelationshipWithPermissionRequest { Ukprn = source.ProviderId, Operation = Operation.CreateCohort });
            await Task.WhenAll(getCohortsTask, hasRelationshipTask);

            var hasExistingCohort = false;

            if(getCohortsTask.Result.Cohorts is not null)
                hasExistingCohort = getCohortsTask.Result.Cohorts.Any();

            var result = new SelectAddDraftApprenticeshipJourneyViewModel
            {
                ProviderId = source.ProviderId,
                HasCreateCohortPermission = hasRelationshipTask.Result,
                HasExistingCohort = hasExistingCohort,
                IsBulkUploadV2Enabled = _featureTogglesService.GetFeatureToggle(ProviderFeature.BulkUploadV2WithoutPrefix).IsEnabled
            };

            return result;
        }
    }
}
