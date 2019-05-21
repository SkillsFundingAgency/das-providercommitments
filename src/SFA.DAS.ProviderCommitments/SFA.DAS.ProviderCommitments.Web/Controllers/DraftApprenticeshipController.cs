using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Models;
using SFA.DAS.ProviderCommitments.Models.ApiModels;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;
using SFA.DAS.ProviderCommitments.Web.Attributes;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{

    [Route("{providerId}/unapproved/{cohortReference}")]
    [Authorize()]
    public class DraftApprenticeshipController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IProviderCommitmentsService _providerCommitmentsService;
        private readonly IMapper<AddDraftApprenticeshipViewModel, AddDraftApprenticeshipToCohortRequest> _addDraftApprenticeshipToCohortRequestMapper;
        private readonly IMapper<EditDraftApprenticeshipDetails, EditDraftApprenticeshipViewModel> _editDraftApprenticeshipDetailsToViewModelMapper;
        private readonly ILinkGenerator _urlHelper;

        public DraftApprenticeshipController(IMediator mediator,
            IProviderCommitmentsService providerCommitmentsService,
            IMapper<AddDraftApprenticeshipViewModel, AddDraftApprenticeshipToCohortRequest> addDraftApprenticeshipToCohortRequestMapper,
            IMapper<EditDraftApprenticeshipDetails, EditDraftApprenticeshipViewModel> editDraftApprenticeshipDetailsToViewModelMapper,
            ILinkGenerator urlHelper)
        {
            _mediator = mediator;
            _providerCommitmentsService = providerCommitmentsService;
            _addDraftApprenticeshipToCohortRequestMapper = addDraftApprenticeshipToCohortRequestMapper;
            _editDraftApprenticeshipDetailsToViewModelMapper = editDraftApprenticeshipDetailsToViewModelMapper;
            _urlHelper = urlHelper;
        }

        [HttpGet]
        [Route("add-apprentice")]
        public async Task<IActionResult> AddDraftApprenticeship(NonReservationsAddDraftApprenticeshipRequest nonReservationsAddDraftApprenticeshipRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new AddDraftApprenticeshipViewModel
            {
                CohortReference = nonReservationsAddDraftApprenticeshipRequest.CohortReference,
                CohortId = nonReservationsAddDraftApprenticeshipRequest.CohortId
            };

            await AddLegalEntityAndCoursesToModel(model);

            return View(model);
        }

        [HttpGet]
        [Route("add-apprentice")]
        [RequireQueryParameter("ReservationId")]
        public async Task<IActionResult> AddDraftApprenticeship(ReservationsAddDraftApprenticeshipRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new AddDraftApprenticeshipViewModel
            {
                CohortReference = request.CohortReference,
                CohortId = request.CohortId,
                StartDate = new MonthYearModel(request.StartMonthYear),
                ReservationId = request.ReservationId,
                CourseCode = request.CourseCode
            };

            await AddLegalEntityAndCoursesToModel(model);

            return View(model);
        }

        [HttpPost]
        [Route("add-apprentice")]
        public async Task<IActionResult> AddDraftApprenticeship(AddDraftApprenticeshipViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await AddLegalEntityAndCoursesToModel(model);
                return View(model);
            }

            var request = _addDraftApprenticeshipToCohortRequestMapper.Map(model);
            request.UserId = User.Upn();

            try
            {
                await _providerCommitmentsService.AddDraftApprenticeshipToCohort(request);
                var cohortDetailsUrl = $"{model.ProviderId}/apprentices/{model.CohortReference}/Details";
                var url = _urlHelper.ProviderApprenticeshipServiceLink(cohortDetailsUrl);
                return Redirect(url);
            }
            catch (CommitmentsApiModelException ex)
            {
                ModelState.AddModelExceptionErrors(ex);
                await AddLegalEntityAndCoursesToModel(model);
                return View(model);
            }
        }

        [HttpGet]
        [Route("edit-apprentice/{DraftApprenticeshipHashedId}")]
        public async Task<IActionResult> EditDraftApprenticeship(EditDraftApprenticeshipRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = _editDraftApprenticeshipDetailsToViewModelMapper.Map(await _providerCommitmentsService.GetDraftApprenticeshipForCohort(request.CohortId.Value, request.DraftApprenticeshipId.Value));
            model.Courses = await GetCourses();

            return View(model);
        }


        private async Task AddLegalEntityAndCoursesToModel(AddDraftApprenticeshipViewModel model)
        {
            var getCohortDetail = _providerCommitmentsService.GetCohortDetail(model.CohortId.Value);
            var getCoursesTask = GetCourses();

            await Task.WhenAll(getCohortDetail, getCoursesTask);

            var cohortDetail = getCohortDetail.Result;

            model.Employer = cohortDetail.LegalEntityName;
            model.Courses = getCoursesTask.Result;
        }

        private async Task<ICourse[]> GetCourses()
        {
            var result = await _mediator.Send(new GetTrainingCoursesQueryRequest { IncludeFrameworks = true });

            return result.TrainingCourses;
        }
    }
}