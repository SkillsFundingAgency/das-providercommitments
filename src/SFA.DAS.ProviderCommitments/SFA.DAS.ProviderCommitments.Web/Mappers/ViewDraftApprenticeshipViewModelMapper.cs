using System.Threading.Tasks;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class ViewDraftApprenticeshipViewModelMapper : IMapper<DraftApprenticeshipRequest, ViewDraftApprenticeshipViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public ViewDraftApprenticeshipViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<ViewDraftApprenticeshipViewModel> Map(DraftApprenticeshipRequest source)
        {
            var draftApprenticeship = await _commitmentsApiClient.GetDraftApprenticeship(source.CohortId, source.DraftApprenticeshipId);

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
                DeliveryModel = draftApprenticeship.DeliveryModel.ToIrregularDescription(),
                IsPortableFlexiJob = draftApprenticeship.DeliveryModel == DeliveryModel.PortableFlexiJob,
                Cost = draftApprenticeship.Cost,
                StartDate = draftApprenticeship.StartDate,
                EndDate = draftApprenticeship.EndDate,
                Reference = draftApprenticeship.Reference,
                TrainingCourseOption = GetCourseOption(draftApprenticeship.TrainingCourseOption),
                TrainingCourseVersion = draftApprenticeship.TrainingCourseVersion,
                EmploymentPrice = draftApprenticeship.EmploymentPrice,
                EmploymentEndDate = draftApprenticeship.EmploymentEndDate,

                HasTrainingCourseOption = draftApprenticeship.HasStandardOptions
            };

            return result;
        }

        private string GetCourseOption(string draftApprenticeshipTrainingCourseOption)
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
