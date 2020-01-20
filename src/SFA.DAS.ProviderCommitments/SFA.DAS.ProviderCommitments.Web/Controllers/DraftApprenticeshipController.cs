using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Authorization.CommitmentPermissions.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
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
        private readonly IMapper<AddDraftApprenticeshipViewModel, AddDraftApprenticeshipRequest> _addDraftApprenticeshipToCohortRequestMapper;
        private readonly IMapper<EditDraftApprenticeshipRequest, EditDraftApprenticeshipViewModel> _editDraftApprenticeshipDetailsToViewModelMapper;
        private readonly IMapper<EditDraftApprenticeshipViewModel, UpdateDraftApprenticeshipRequest> _updateDraftApprenticeshipRequestMapper;
        private readonly ILinkGenerator _urlHelper;
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public DraftApprenticeshipController(IMediator mediator,
            IMapper<AddDraftApprenticeshipViewModel, AddDraftApprenticeshipRequest> addDraftApprenticeshipToCohortRequestMapper,
            IMapper<EditDraftApprenticeshipRequest, EditDraftApprenticeshipViewModel> editDraftApprenticeshipDetailsToViewModelMapper,
            IMapper<EditDraftApprenticeshipViewModel, UpdateDraftApprenticeshipRequest> updateDraftApprenticeshipRequestMapper,
            ILinkGenerator urlHelper, ICommitmentsApiClient commitmentsApiClient)
        {
            _mediator = mediator;
            _addDraftApprenticeshipToCohortRequestMapper = addDraftApprenticeshipToCohortRequestMapper;
            _editDraftApprenticeshipDetailsToViewModelMapper = editDraftApprenticeshipDetailsToViewModelMapper;
            _updateDraftApprenticeshipRequestMapper = updateDraftApprenticeshipRequestMapper;
            _urlHelper = urlHelper;
            _commitmentsApiClient = commitmentsApiClient;
        }

        [HttpGet]
        [Route("add")]
        [RequireQueryParameter("ReservationId")]
        public async Task<IActionResult> AddDraftApprenticeship(ReservationsAddDraftApprenticeshipRequest request)
        {
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
            var request = await _addDraftApprenticeshipToCohortRequestMapper.Map(model);
            request.UserId = User.Upn();

            await _commitmentsApiClient.AddDraftApprenticeship(model.CohortId.Value, request);
            
            var cohortDetailsUrl = $"{model.ProviderId}/apprentices/{model.CohortReference}/Details";
            var url = _urlHelper.ProviderApprenticeshipServiceLink(cohortDetailsUrl);
            return Redirect(url);
        }

        [HttpGet]
        [Route("{DraftApprenticeshipHashedId}/edit")]
        public async Task<IActionResult> EditDraftApprenticeship(EditDraftApprenticeshipRequest request)
        {
            var model = await _editDraftApprenticeshipDetailsToViewModelMapper.Map(request);

            model.ProviderId = request.ProviderId;
            await AddLegalEntityAndCoursesToModel(model);
            return View(model);
        }

        [HttpPost]
        [Route("{DraftApprenticeshipHashedId}/edit")]
        public async Task<IActionResult> EditDraftApprenticeship(EditDraftApprenticeshipViewModel model)
        {
            var updateRequest = await _updateDraftApprenticeshipRequestMapper.Map(model);
            await _commitmentsApiClient.UpdateDraftApprenticeship(model.CohortId.Value, model.DraftApprenticeshipId.Value, updateRequest);
            var cohortDetailsUrl = $"{model.ProviderId}/apprentices/{model.CohortReference}/Details";
            var url = _urlHelper.ProviderApprenticeshipServiceLink(cohortDetailsUrl);
            return Redirect(url);
        }

        private async Task AddLegalEntityAndCoursesToModel(DraftApprenticeshipViewModel model)
        {
            var cohortDetail = await _commitmentsApiClient.GetCohort(model.CohortId.Value);

            var courses = await GetCourses(cohortDetail);

            model.Employer = cohortDetail.LegalEntityName;
            model.Courses = courses;
        }

        private async Task<ITrainingProgramme[]> GetCourses(GetCohortResponse cohortDetails)
        {
            var result = await _mediator.Send(new GetTrainingCoursesQueryRequest { IncludeFrameworks = !cohortDetails.IsFundedByTransfer });

            return result.TrainingCourses;
        }
    }
}