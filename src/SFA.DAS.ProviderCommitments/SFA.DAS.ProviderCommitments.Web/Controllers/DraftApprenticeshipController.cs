using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.CommitmentPermissions.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse;
using SFA.DAS.ProviderCommitments.Features;
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
    [Route("{providerId}/unapproved/{cohortReference}/apprentices")]
    [DasAuthorize(CommitmentOperation.AccessCohort)]
    public class DraftApprenticeshipController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IProviderCommitmentsService _providerCommitmentsService;
        private readonly IMapper<AddDraftApprenticeshipViewModel, AddDraftApprenticeshipRequest> _addDraftApprenticeshipToCohortRequestMapper;
        private readonly IMapper<EditDraftApprenticeshipDetails, EditDraftApprenticeshipViewModel> _editDraftApprenticeshipDetailsToViewModelMapper;
        private readonly IMapper<EditDraftApprenticeshipViewModel, UpdateDraftApprenticeshipRequest> _updateDraftApprenticeshipRequestMapper;
        private readonly ILinkGenerator _urlHelper;
        private readonly ILogger<DraftApprenticeshipController> _logger;

        public DraftApprenticeshipController(IMediator mediator,
            IProviderCommitmentsService providerCommitmentsService,
            IMapper<AddDraftApprenticeshipViewModel, AddDraftApprenticeshipRequest> addDraftApprenticeshipToCohortRequestMapper,
            IMapper<EditDraftApprenticeshipDetails, EditDraftApprenticeshipViewModel> editDraftApprenticeshipDetailsToViewModelMapper,
            IMapper<EditDraftApprenticeshipViewModel, UpdateDraftApprenticeshipRequest> updateDraftApprenticeshipRequestMapper,
            ILinkGenerator urlHelper,
            ILogger<DraftApprenticeshipController> logger)
        {
            _mediator = mediator;
            _providerCommitmentsService = providerCommitmentsService;
            _addDraftApprenticeshipToCohortRequestMapper = addDraftApprenticeshipToCohortRequestMapper;
            _editDraftApprenticeshipDetailsToViewModelMapper = editDraftApprenticeshipDetailsToViewModelMapper;
            _updateDraftApprenticeshipRequestMapper = updateDraftApprenticeshipRequestMapper;
            _urlHelper = urlHelper;
            _logger = logger;
        }

        [HttpGet]
        [Route("add")]
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
        [Route("add")]
        [RequireQueryParameter("ReservationId")]
        [DasAuthorize(ProviderFeature.Reservations)]
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
        [Route("add")]
        public async Task<IActionResult> AddDraftApprenticeship(AddDraftApprenticeshipViewModel model)
        {
            LogModelState($"Entered {nameof(AddDraftApprenticeship)}");

            if (!ModelState.IsValid)
            {
                await AddLegalEntityAndCoursesToModel(model);
                return View(model);
            }

            var request = _addDraftApprenticeshipToCohortRequestMapper.Map(model);
            request.UserId = User.Upn();

            try
            {
                await _providerCommitmentsService.AddDraftApprenticeshipToCohort(model.CohortId.Value, request);
                var cohortDetailsUrl = $"{model.ProviderId}/apprentices/{model.CohortReference}/Details";
                var url = _urlHelper.ProviderApprenticeshipServiceLink(cohortDetailsUrl);
                _logger.Log(LogLevel.Debug, $"Redirecting to URL:{url}");
                return Redirect(url);
            }
            catch (CommitmentsApiModelException ex)
            {
                _logger.Log(LogLevel.Debug, $"Encountered exception {ex.GetType().Name} - {ex.Message} - {ex.StackTrace}");
                ModelState.AddModelExceptionErrors(ex);
                await AddLegalEntityAndCoursesToModel(model);
                return View(model);
            }
        }

        [HttpGet]
        [Route("{DraftApprenticeshipHashedId}/edit")]
        public async Task<IActionResult> EditDraftApprenticeship(EditDraftApprenticeshipRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = _editDraftApprenticeshipDetailsToViewModelMapper.Map(
                await _providerCommitmentsService.GetDraftApprenticeshipForCohort(request.CohortId.Value,
                    request.DraftApprenticeshipId.Value));

            await AddLegalEntityAndCoursesToModel(model);

            return View(model);
        }

        [HttpPost]
        [Route("{DraftApprenticeshipHashedId}/edit")]
        public async Task<IActionResult> EditDraftApprenticeship(EditDraftApprenticeshipViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await AddLegalEntityAndCoursesToModel(model);
                return View(model);
            }

            var updateRequest = _updateDraftApprenticeshipRequestMapper.Map(model);

            try
            {
                await _providerCommitmentsService.UpdateDraftApprenticeship(model.CohortId.Value, model.DraftApprenticeshipId.Value, updateRequest);
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

        private void LogModelState(string message)
        {
            var sb = new StringBuilder();

            sb.Append(message);
            sb.AppendLine($" ModelState:IsValid {ModelState.IsValid} ModelState.ErrorCount: {ModelState.ErrorCount} (invalid properties listed below)");

            foreach (var state in ModelState.Where(s => s.Value.Errors.Count > 0))
            {
                sb.Append($"Name: {state.Key}");
                sb.Append($" Count: {state.Value.Errors.Count}");
                foreach (var error in state.Value.Errors)
                {
                    sb.Append($"{error.ErrorMessage} : {error.Exception}");
                }
            }

            _logger.Log(LogLevel.Debug, sb.ToString());
        }

        private async Task AddLegalEntityAndCoursesToModel(DraftApprenticeshipViewModel model)
        {
            var cohortDetail = await _providerCommitmentsService.GetCohortDetail(model.CohortId.Value);
            var courses = await GetCourses(cohortDetail);

            model.Employer = cohortDetail.LegalEntityName;
            model.Courses = courses;
        }

        private async Task<ICourse[]> GetCourses(CohortDetails cohortDetails)
        {
            var result = await _mediator.Send(new GetTrainingCoursesQueryRequest { IncludeFrameworks = !cohortDetails.IsFundedByTransfer });

            return result.TrainingCourses;
        }
    }
}