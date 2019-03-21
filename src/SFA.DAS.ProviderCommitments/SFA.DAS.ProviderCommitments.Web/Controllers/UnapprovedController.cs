using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse;
using SFA.DAS.ProviderCommitments.Extensions;
using SFA.DAS.ProviderCommitments.Models;
using SFA.DAS.ProviderCommitments.Queries.GetAccountLegalEntity;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourse;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}/unapproved")]
    [Authorize()]
    public class UnapprovedController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ICreateCohortRequestMapper _createCohortRequestMapper;
        private readonly ILinkGenerator _urlHelper;

        public UnapprovedController(IMediator mediator,
            ICreateCohortRequestMapper createCohortRequestMapper,
            ILinkGenerator urlHelper)
        {
            _mediator = mediator;
            _createCohortRequestMapper = createCohortRequestMapper;
            _urlHelper = urlHelper;
        }

        [HttpGet]
        [Route("add-apprentice")]
        public async Task<IActionResult> AddDraftApprenticeship(AddDraftApprenticeshipRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new AddDraftApprenticeshipViewModel
            {
                AccountLegalEntity = request.AccountLegalEntity,
                StartDate = new MonthYearModel(request.StartMonthYear),
                ReservationId = request.ReservationId,
                CourseCode = request.CourseCode
            };

            await AddEmployerAndCoursesToModel(model);

            return View(model);
        }

        [HttpPost]
        [Route("add-apprentice")]
        public async Task<IActionResult> AddDraftApprenticeship(AddDraftApprenticeshipViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await AddEmployerAndCoursesToModel(model);
                return View(model);
            }

            var request = _createCohortRequestMapper.Map(model);
            request.UserId = User.Upn();
            var response = await _mediator.Send(request);

            var cohortDetailsUrl = $"{model.ProviderId}/apprentices/{response.CohortReference}/Details";
            var url = _urlHelper.ProviderApprenticeshipServiceLink(cohortDetailsUrl);
            return Redirect(url);
        }

        private async Task AddEmployerAndCoursesToModel(AddDraftApprenticeshipViewModel model)
        {
            var getEmployerTask =
                GetEmployerIfRequired(model.AccountLegalEntity.AccountLegalEntityId);

            var getCoursesTask = GetCourses();

            await Task.WhenAll(getEmployerTask, getCoursesTask);

            model.Employer = getEmployerTask.Result?.LegalEntityName;
            model.Courses = getCoursesTask.Result;
        }

        private Task<GetAccountLegalEntityResponse>  GetEmployerIfRequired(long? accountLegalEntityId)
        {
            if (!accountLegalEntityId.HasValue)
            {
                return Task.FromResult((GetAccountLegalEntityResponse) null);
            }

            return _mediator.Send(new GetAccountLegalEntityRequest
            {
                EmployerAccountLegalEntityId = accountLegalEntityId.Value
            });
        }

        private Task<GetTrainingCourseResponse> GetTrainingCourseIfRequired(string trainingCode)
        {
            if (string.IsNullOrWhiteSpace(trainingCode))
            {
                return Task.FromResult((GetTrainingCourseResponse)null);
            }

            return _mediator.Send(new GetTrainingCourseRequest { CourseCode = trainingCode});
        }

        private async Task<ICourse[]> GetCourses()
        {
            var result = await _mediator.Send(new GetTrainingCoursesQueryRequest { IncludeFrameworks = true });

            return result.TrainingCourses;
        }
    }
}