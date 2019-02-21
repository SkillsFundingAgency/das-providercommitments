﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderCommitments.Models;
using SFA.DAS.ProviderCommitments.Queries.GetEmployer;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourse;
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
        public async Task<IActionResult> Index(EditApprenticeshipRequest request)
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

            var model = new EditApprenticeshipViewModel
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

        private Task<Course[]> GetCourses()
        {
            return Task.FromResult(new Course[]
            {
                new Course("0001", "Basic appreciation of silence"),
                new Course("0002", "Advanced understanding of darkness"),
            });
        }
    }
}