using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class ViewDraftApprenticeshipViewModelMapper : IMapper<DraftApprenticeshipRequest, ViewDraftApprenticeshipViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IOuterApiClient _outerApiClient;

        public ViewDraftApprenticeshipViewModelMapper(ICommitmentsApiClient commitmentsApiClient, IOuterApiClient outerApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _outerApiClient = outerApiClient;
        }

        public async Task<ViewDraftApprenticeshipViewModel> Map(DraftApprenticeshipRequest source)
        {
            var draftApprenticeship = await _outerApiClient.Get<GetViewDraftApprenticeshipResponse>(new GetViewDraftApprenticeshipRequest(source.ProviderId, source.CohortId, source.DraftApprenticeshipId));

            var trainingCourse = string.IsNullOrWhiteSpace(draftApprenticeship.CourseCode) ? null
                : await _commitmentsApiClient.GetTrainingProgramme(draftApprenticeship.CourseCode);

            var result = new ViewDraftApprenticeshipViewModel
            {
                ProviderId = source.ProviderId,
                CohortReference = source.CohortReference,
                FirstName = draftApprenticeship.FirstName,
                LastName = draftApprenticeship.LastName,
                Email = draftApprenticeship.Email,
                Uln = draftApprenticeship.Uln,
                DateOfBirth = draftApprenticeship.DateOfBirth,
                TrainingCourse = trainingCourse?.TrainingProgramme.Name,
                DeliveryModel = draftApprenticeship.DeliveryModel,
                TrainingPrice = draftApprenticeship.TrainingPrice,
                EndPointAssessmentPrice = draftApprenticeship.EndPointAssessmentPrice,
                Cost = draftApprenticeship.Cost,
                EmploymentPrice = draftApprenticeship.EmploymentPrice,
                StartDate = draftApprenticeship.StartDate,
                EmploymentEndDate = draftApprenticeship.EmploymentEndDate,
                EndDate = draftApprenticeship.EndDate,
                Reference = draftApprenticeship.Reference,
                TrainingCourseOption = GetCourseOption(draftApprenticeship.TrainingCourseOption),
                TrainingCourseVersion = draftApprenticeship.TrainingCourseVersion,
                HasTrainingCourseOption = draftApprenticeship.HasStandardOptions,
                RecognisePriorLearning = draftApprenticeship.RecognisePriorLearning,
                RecognisingPriorLearningStillNeedsToBeConsidered = draftApprenticeship.RecognisingPriorLearningStillNeedsToBeConsidered,
                RecognisingPriorLearningExtendedStillNeedsToBeConsidered = draftApprenticeship.RecognisingPriorLearningExtendedStillNeedsToBeConsidered,
                DurationReducedBy = draftApprenticeship.DurationReducedBy,
                PriceReducedBy = draftApprenticeship.PriceReducedBy,
                DurationReducedByHours = draftApprenticeship.DurationReducedByHours,
                IsDurationReducedByRpl = draftApprenticeship.IsDurationReducedByRpl,
                TrainingTotalHours = draftApprenticeship.TrainingTotalHours
            };

            return result;
        }

        private static string GetCourseOption(string draftApprenticeshipTrainingCourseOption)
        {
            return draftApprenticeshipTrainingCourseOption switch
            {
                null => string.Empty,
                "" => "To be confirmed",
                _ => draftApprenticeshipTrainingCourseOption
            };
        }
    }
}
