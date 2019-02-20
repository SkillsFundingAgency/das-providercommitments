using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderCommitments.Queries.GetEmployer;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourse;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    public class ApprenticeshipsController : Controller
    {
        private readonly IMediator _mediator;

        public ApprenticeshipsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int reservationId, string employerId, string startMonthYear, string trainingCode)
        {
            var getEmployerTask = GetEmployerIfRequired(employerId);
            var getTrainingCourseTask = GetTrainingCourseIfRequired(trainingCode);

            await Task.WhenAll(getEmployerTask, getTrainingCourseTask);

            var model = new EditApprenticeshipViewModel(startMonthYear)
            {
                ReservationId = reservationId,
                CourseCode = trainingCode,
                CourseName = getTrainingCourseTask.Result?.CourseName,
                Employer = getEmployerTask.Result?.EmployerName
            };

            return View(model);
        }
        
        private Task<GetEmployerResponse>  GetEmployerIfRequired(string employerId)
        {
            if (string.IsNullOrWhiteSpace(employerId))
            {
                return Task.FromResult((GetEmployerResponse) null);
            }

            return _mediator.Send(new GetEmployerRequest {EmployerId = employerId});
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