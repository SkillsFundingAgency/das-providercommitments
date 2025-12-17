using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class ViewStandardOptionsViewModelMapper : IMapper<SelectOptionsRequest, ViewSelectOptionsViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IOuterApiClient _outerApiClient;

        public ViewStandardOptionsViewModelMapper (ICommitmentsApiClient commitmentsApiClient, IOuterApiClient outerApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _outerApiClient = outerApiClient;
        }
        public async Task<ViewSelectOptionsViewModel> Map(SelectOptionsRequest source)
        {            
            var apiRequest = new GetEditDraftApprenticeshipRequest(source.ProviderId, (long)source.CohortId, (long)source.DraftApprenticeshipId, null);
            var draftApprenticeship = await _outerApiClient.Get<GetEditDraftApprenticeshipResponse>(apiRequest);

            var trainingProgramme = await _commitmentsApiClient.GetTrainingProgrammeVersionByStandardUId(draftApprenticeship.StandardUId);

            return new ViewSelectOptionsViewModel
            {
                CohortId = source.CohortId,
                CohortReference = source.CohortReference,
                ProviderId = source.ProviderId,
                DraftApprenticeshipId = source.DraftApprenticeshipId,
                DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId,
                TrainingCourseName = draftApprenticeship.CourseName,
                TrainingCourseVersion = draftApprenticeship.TrainingCourseVersion,
                Options = trainingProgramme.TrainingProgramme.Options != null ? trainingProgramme.TrainingProgramme.Options.ToList() : new List<string>(),
                StandardPageUrl = trainingProgramme.TrainingProgramme.StandardPageUrl,
                SelectedOption = draftApprenticeship.TrainingCourseOption == string.Empty ? "-1" : draftApprenticeship.TrainingCourseOption,
                HasSelectedRpl = draftApprenticeship.RecognisePriorLearning,
                ApprenticeshipStartDate = draftApprenticeship.StartDate,
                LearnerDataId = draftApprenticeship.LearnerDataId
            };
        }
    }
}