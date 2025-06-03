using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class SelectAddDraftApprenticeshipJourneyRequestViewModelMapper : IMapper<SelectAddDraftApprenticeshipJourneyRequest, SelectAddDraftApprenticeshipJourneyViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IApprovalsOuterApiClient _approvalsOuterApiClient;

        public SelectAddDraftApprenticeshipJourneyRequestViewModelMapper(
            ICommitmentsApiClient commitmentApiClient,
            IApprovalsOuterApiClient approvalsOuterApiClient)
        {
            _approvalsOuterApiClient = approvalsOuterApiClient;
            _commitmentsApiClient = commitmentApiClient;
        }

        public async Task<SelectAddDraftApprenticeshipJourneyViewModel> Map(SelectAddDraftApprenticeshipJourneyRequest source)
        {
            var getCohortsTask = _commitmentsApiClient.GetCohorts(new GetCohortsRequest { ProviderId = source.ProviderId });
            var hasRelationshipTask = _approvalsOuterApiClient.GetHasRelationshipWithPermission(source.ProviderId);
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
                HasCreateCohortPermission = hasRelationshipTask.Result.HasPermission,
                HasExistingCohort = hasExistingCohort,
                UseLearnerData = source.UseLearnerData
            };

            return result;
        }
    }
}
