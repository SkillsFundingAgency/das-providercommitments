using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderCommitments.Queries.GetEmployer;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourse;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}/unapproved")]
    [AllowAnonymous]
    public class UnapprovedController : Controller
    {
        private readonly IMediator _mediator;

        public UnapprovedController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("add-apprentice")]
        public async Task<IActionResult> Index(Guid reservationId, string employerAccountPublicHashedId, string employerAccountLegalEntityPublicHashedId, string startMonthYear, string courseCode)
        {
            var getEmployerTask = GetEmployerIfRequired(employerAccountPublicHashedId, employerAccountLegalEntityPublicHashedId);
            var getTrainingCourseTask = GetTrainingCourseIfRequired(courseCode);

            await Task.WhenAll(getEmployerTask, getTrainingCourseTask);

            var model = new EditApprenticeshipViewModel(startMonthYear)
            {
                ReservationId = reservationId,
                CourseCode = courseCode,
                CourseName = getTrainingCourseTask.Result?.CourseName,
                Employer = getEmployerTask.Result?.EmployerName
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
    }
}