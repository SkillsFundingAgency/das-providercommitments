using System.Threading.Tasks;
using MediatR;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class SelectCourseViewModelMapperHelper : ISelectCourseViewModelMapperHelper
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IMediator _mediator;

        public SelectCourseViewModelMapperHelper(ICommitmentsApiClient commitmentsApiClient, IMediator mediator)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _mediator = mediator;
        }

        public async Task<SelectCourseViewModel> Map(string courseCode, long accountLegalEntityId, bool? isOnFlexiPaymentsPilot)
        {
            var ale = await _commitmentsApiClient.GetAccountLegalEntity(accountLegalEntityId);

            return new SelectCourseViewModel
            {
                CourseCode = courseCode,
                Courses = await GetCourses(ale.LevyStatus),
                IsOnFlexiPaymentsPilot = isOnFlexiPaymentsPilot
            };
        }

        private async Task<TrainingProgramme[]> GetCourses(ApprenticeshipEmployerType levyStatus)
        {
            var result = await _mediator.Send(new GetTrainingCoursesQueryRequest { IncludeFrameworks = levyStatus != ApprenticeshipEmployerType.NonLevy });
            return result.TrainingCourses;
        }

    }
}