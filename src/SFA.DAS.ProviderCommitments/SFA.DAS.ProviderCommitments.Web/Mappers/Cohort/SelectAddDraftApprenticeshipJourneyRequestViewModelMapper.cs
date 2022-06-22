using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
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

        public SelectAddDraftApprenticeshipJourneyRequestViewModelMapper(
            ICommitmentsApiClient commitmentApiClient,
            IProviderRelationshipsApiClient providerRelationshipsApiClient)
        {
            _providerRelationshipsApiClient = providerRelationshipsApiClient;
            _commitmentsApiClient = commitmentApiClient;
        }

        public async Task<SelectAddDraftApprenticeshipJourneyViewModel> Map(SelectAddDraftApprenticeshipJourneyRequest source)
        {
            var getCohortsTask = _commitmentsApiClient.GetCohorts(new GetCohortsRequest { ProviderId = source.ProviderId });
            var hasRelationshipTask = _providerRelationshipsApiClient.HasRelationshipWithPermission(
                new HasRelationshipWithPermissionRequest
                {
                    Ukprn = source.ProviderId,
                    Operation = Operation.CreateCohort
                }, CancellationToken.None);
            await Task.WhenAll(getCohortsTask, hasRelationshipTask);

            var hasExistingCohort = false;

            if (getCohortsTask.Result.Cohorts != null)
            {
                var cohorts = getCohortsTask.Result.Cohorts;
                var cohortsInDraftCount = cohorts.Count(x => x.GetStatus() == CohortStatus.Draft || x.GetStatus() == CohortStatus.Review);

                hasExistingCohort = cohortsInDraftCount > 0;
            }
             
            var result = new SelectAddDraftApprenticeshipJourneyViewModel
            {
                ProviderId = source.ProviderId,
                HasCreateCohortPermission = hasRelationshipTask.Result,
                HasExistingCohort = hasExistingCohort
            };

            return result;
        }
    }
}
