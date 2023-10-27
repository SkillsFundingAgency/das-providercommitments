using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class DeleteCohortRequestViewModelMapper : IMapper<DeleteCohortRequest, DeleteCohortViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public DeleteCohortRequestViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<DeleteCohortViewModel> Map(DeleteCohortRequest source)
        {
            string TextOrDefault(string txt) => !string.IsNullOrEmpty(txt) ? txt : "without training course details";

            var cohortTask = _commitmentsApiClient.GetCohort(source.CohortId);
            var draftApprenticeshipsTask = _commitmentsApiClient.GetDraftApprenticeships(source.CohortId);

            await Task.WhenAll(cohortTask, draftApprenticeshipsTask);

            var cohort = await cohortTask;
            var draftApprenticeships = (await draftApprenticeshipsTask).DraftApprenticeships.ToList();

            var programmeSummary = draftApprenticeships
    .GroupBy(m => m.CourseName)
    .Select(m => $"{m.Count()} {TextOrDefault(m.Key)}")
    .ToList();

            return new DeleteCohortViewModel
            {
                ProviderId = source.ProviderId,
                EmployerAccountName = cohort.LegalEntityName,
                CohortReference = source.CohortReference,
                NumberOfApprenticeships = draftApprenticeships.Count,
                ApprenticeshipTrainingProgrammes = programmeSummary
            };
        }
    }
}
