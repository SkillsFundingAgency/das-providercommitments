using System.Threading.Tasks;
using MediatR;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourse;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.CommitmentsV2.Shared.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class AddDraftApprenticeshipViewModelMapper : IMapper<CreateCohortWithDraftApprenticeshipRequest, AddDraftApprenticeshipViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IMediator _mediator;

        public AddDraftApprenticeshipViewModelMapper(ICommitmentsApiClient commitmentsApiClient, IMediator mediator)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _mediator = mediator;
        }

        public async Task<AddDraftApprenticeshipViewModel> Map(CreateCohortWithDraftApprenticeshipRequest source)
        {
            var aleTask = _commitmentsApiClient.GetAccountLegalEntity(source.AccountLegalEntityId);
            var courseTask = GetCourse(source.CourseCode);

            await Task.WhenAll(aleTask, courseTask);

            return new AddDraftApprenticeshipViewModel
            {
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                AccountLegalEntityId = source.AccountLegalEntityId,
                StartDate = new MonthYearModel(source.StartMonthYear),
                ReservationId = source.ReservationId.Value,
                CourseCode = source.CourseCode,
                CourseName = courseTask.Result.CourseName,
                Courses = await GetCourses(aleTask.Result.LevyStatus),
                Employer = aleTask.Result.LegalEntityName
            };
        }

        private async Task<TrainingProgramme[]> GetCourses(ApprenticeshipEmployerType levyStatus)
        {
            var result = await _mediator.Send(new GetTrainingCoursesQueryRequest { IncludeFrameworks = levyStatus != ApprenticeshipEmployerType.NonLevy });
            return result.TrainingCourses;
        }

        private async Task<GetTrainingCourseResponse> GetCourse(string courseCode)
            => await _mediator.Send(new GetTrainingCourseRequest { CourseCode = courseCode });            
    }
}
