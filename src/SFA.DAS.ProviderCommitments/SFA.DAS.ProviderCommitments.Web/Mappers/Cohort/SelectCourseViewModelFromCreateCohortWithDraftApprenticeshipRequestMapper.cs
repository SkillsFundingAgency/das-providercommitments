using MediatR;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class SelectCourseViewModelFromCreateCohortWithDraftApprenticeshipRequestMapper : IMapper<CreateCohortWithDraftApprenticeshipRequest, SelectCourseViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IMediator _mediator;

        public SelectCourseViewModelFromCreateCohortWithDraftApprenticeshipRequestMapper(ICommitmentsApiClient commitmentsApiClient, IMediator mediator)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _mediator = mediator;
        }

        public async Task<SelectCourseViewModel> Map(CreateCohortWithDraftApprenticeshipRequest source)
        {
            var ale = await _commitmentsApiClient.GetAccountLegalEntity(source.AccountLegalEntityId);

            return new SelectCourseViewModel
            {
                //ReservationId = source.ReservationId,
                //EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                CourseCode = source.CourseCode,
                //StartMonthYear = source.StartMonthYear,
                Courses = await GetCourses(ale.LevyStatus),
            };
        }

        private async Task<TrainingProgramme[]> GetCourses(ApprenticeshipEmployerType levyStatus)
        {
            var result = await _mediator.Send(new GetTrainingCoursesQueryRequest { IncludeFrameworks = levyStatus != ApprenticeshipEmployerType.NonLevy });
            return result.TrainingCourses;
        }
    }
}