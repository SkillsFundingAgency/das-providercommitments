using MediatR;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;
using SFA.DAS.Authorization.Services;
using SFA.DAS.ProviderCommitments.Features;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class AddDraftApprenticeshipViewModelMapper : IMapper<CreateCohortWithDraftApprenticeshipRequest, AddDraftApprenticeshipViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IMediator _mediator;
        private readonly IAuthorizationService _authorizationService;

        public AddDraftApprenticeshipViewModelMapper(ICommitmentsApiClient commitmentsApiClient, IMediator mediator, IAuthorizationService authorizationService)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _mediator = mediator;
            _authorizationService = authorizationService;
        }

        public async Task<AddDraftApprenticeshipViewModel> Map(CreateCohortWithDraftApprenticeshipRequest source)
        {
            var ale = await _commitmentsApiClient.GetAccountLegalEntity(source.AccountLegalEntityId);

            return new AddDraftApprenticeshipViewModel
            {
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                AccountLegalEntityId = source.AccountLegalEntityId,
                StartDate = new MonthYearModel(source.StartMonthYear),
                ReservationId = source.ReservationId.Value,
                CourseCode = source.CourseCode,
                DeliveryModel = source.DeliveryModel,
                Courses = _authorizationService.IsAuthorized(ProviderFeature.DeliveryModel) == false ? await GetCourses(ale.LevyStatus) : null,
                Employer = ale.LegalEntityName
            };
        }

        private async Task<TrainingProgramme[]> GetCourses(ApprenticeshipEmployerType levyStatus)
        {
            var result = await _mediator.Send(new GetTrainingCoursesQueryRequest { IncludeFrameworks = levyStatus != ApprenticeshipEmployerType.NonLevy });
            return result.TrainingCourses;
        }
    }
}