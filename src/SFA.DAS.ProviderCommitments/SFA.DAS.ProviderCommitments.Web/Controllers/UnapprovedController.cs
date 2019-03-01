using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse;
using SFA.DAS.ProviderCommitments.Models;
using SFA.DAS.ProviderCommitments.Queries.GetEmployer;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourse;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}/unapproved")]
    [Authorize()]
    public class UnapprovedController : Controller
    {
        private readonly IMediator _mediator;

        public UnapprovedController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("add-apprentice")]
        public async Task<IActionResult> AddDraftApprenticeship(AddDraftApprenticeshipRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var getEmployerTask = GetEmployerIfRequired(
                request.EmployerAccountPublicHashedId,
                request.EmployerAccountLegalEntityPublicHashedId);

            var getTrainingCourseTask = GetTrainingCourseIfRequired(request.CourseCode);

            var getCoursesTask = GetCourses();

            await Task.WhenAll(getEmployerTask, getTrainingCourseTask, getCoursesTask);

            var model = new AddDraftApprenticeshipViewModel
            {
                StartDate = new MonthYearModel(request.StartMonthYear),
                ReservationId = request.ReservationId,
                CourseCode = request.CourseCode,
                CourseName = getTrainingCourseTask.Result?.CourseName,
                Employer = getEmployerTask.Result?.EmployerName,
                Courses = getCoursesTask.Result
            };
            return View(model);
        }

        private Task<GetEmployerResponse>  GetEmployerIfRequired(string employerAccountPublicHashedId, string employerAccountLegalEntityPublicHashedId)
        {
            if (string.IsNullOrWhiteSpace(employerAccountPublicHashedId))
            {
                return Task.FromResult((GetEmployerResponse) null);
            }

            return _mediator.Send(new GetEmployerRequest
            {
                EmployerAccountPublicHashedId = employerAccountPublicHashedId,
                EmployerAccountLegalEntityPublicHashedId = employerAccountLegalEntityPublicHashedId
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