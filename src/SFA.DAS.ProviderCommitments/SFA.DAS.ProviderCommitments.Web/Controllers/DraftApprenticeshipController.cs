using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Authorization.CommitmentPermissions.Options;
using SFA.DAS.Commitments.Shared.Extensions;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.Commitments.Shared.Models;
using SFA.DAS.Commitments.Shared.Models.ApprenticeshipCourse;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Features;
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
        private readonly ICommitmentsService _commitmentsService;
        private readonly IMapper<AddDraftApprenticeshipViewModel, AddDraftApprenticeshipRequest> _addDraftApprenticeshipToCohortRequestMapper;
        private readonly IMapper<EditDraftApprenticeshipDetails, EditDraftApprenticeshipViewModel> _editDraftApprenticeshipDetailsToViewModelMapper;
        private readonly IMapper<EditDraftApprenticeshipViewModel, UpdateDraftApprenticeshipRequest> _updateDraftApprenticeshipRequestMapper;
        private readonly ILinkGenerator _urlHelper;

        public DraftApprenticeshipController(IMediator mediator,
            ICommitmentsService commitmentsService,
            IMapper<AddDraftApprenticeshipViewModel, AddDraftApprenticeshipRequest> addDraftApprenticeshipToCohortRequestMapper,
            IMapper<EditDraftApprenticeshipDetails, EditDraftApprenticeshipViewModel> editDraftApprenticeshipDetailsToViewModelMapper,
            IMapper<EditDraftApprenticeshipViewModel, UpdateDraftApprenticeshipRequest> updateDraftApprenticeshipRequestMapper,
            ILinkGenerator urlHelper)
        {
            _mediator = mediator;
            _commitmentsService = commitmentsService;
            _addDraftApprenticeshipToCohortRequestMapper = addDraftApprenticeshipToCohortRequestMapper;
            _editDraftApprenticeshipDetailsToViewModelMapper = editDraftApprenticeshipDetailsToViewModelMapper;
            _updateDraftApprenticeshipRequestMapper = updateDraftApprenticeshipRequestMapper;
            _urlHelper = urlHelper;
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
                ProviderId = nonReservationsAddDraftApprenticeshipRequest.ProviderId,
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
                ProviderId = request.ProviderId,
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
            if (!ModelState.IsValid)
            {
                await AddLegalEntityAndCoursesToModel(model);
                return View(model);
            }

            var request = await _addDraftApprenticeshipToCohortRequestMapper.Map(model);
            request.UserId = User.Upn();

            try
            {
                await _commitmentsService.AddDraftApprenticeshipToCohort(model.CohortId.Value, request);
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
        [Route("{DraftApprenticeshipHashedId}/edit")]
        public async Task<IActionResult> EditDraftApprenticeship(EditDraftApprenticeshipRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = await _editDraftApprenticeshipDetailsToViewModelMapper.Map(
                await _commitmentsService.GetDraftApprenticeshipForCohort(
                    request.CohortId.Value,
                    request.DraftApprenticeshipId.Value));

            model.ProviderId = request.ProviderId;

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

            var updateRequest = await _updateDraftApprenticeshipRequestMapper.Map(model);

            try
            {
                await _commitmentsService.UpdateDraftApprenticeship(model.CohortId.Value, model.DraftApprenticeshipId.Value, updateRequest);
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

        private async Task AddLegalEntityAndCoursesToModel(DraftApprenticeshipViewModel model)
        {
            var cohortDetail = await _commitmentsService.GetCohortDetail(model.CohortId.Value);
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