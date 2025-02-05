using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}/unapproved/[controller]")]
    public class OverlappingTrainingDateRequestController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IModelMapper _modelMapper;
        private readonly ILinkGenerator _urlHelper;
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IAuthenticationService _authenticationService;
        private readonly IOuterApiService _outerApiService;

        public OverlappingTrainingDateRequestController(IMediator mediator,
            IModelMapper modelMapper,
            ILinkGenerator urlHelper,
            ICommitmentsApiClient commitmentsApiClient,
            IAuthenticationService authenticationService,
            IOuterApiService outerApiService
            )
        {
            _mediator = mediator;
            _modelMapper = modelMapper;
            _urlHelper = urlHelper;
            _commitmentsApiClient = commitmentsApiClient;
            _authenticationService = authenticationService;
            _outerApiService = outerApiService;
        }

        [HttpGet]
        [Route("overlap-options-change-employer")]
        public IActionResult OverlapOptionsForChangeEmployer(OverlapOptionsForChangeEmployerRequest request)
        {
            var viewModel = new OverlapOptionsForChangeEmployerViewModel
            {
                DraftApprenticeshipHashedId = request.ApprenticeshipHashedId,
                ApprenticeshipId = request.ApprenticeshipId,
                ApprenticeshipHashedId = request.ApprenticeshipHashedId,
                ProviderId = request.ProviderId,
                CacheKey = request.CacheKey,
                Status = request.Status
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("overlap-options-change-employer")]
        public async Task<IActionResult> OverlapOptionsForChangeEmployer(OverlapOptionsForChangeEmployerViewModel viewModel)
        {
            if (viewModel.OverlapOptions != OverlapOptions.SendStopRequest)
            {
                return RedirectToAction(ControllerConstants.ApprenticeController.Actions.Index, ControllerConstants.ApprenticeController.Name, new { viewModel.ProviderId });
            }

            var request = await _modelMapper.Map<ChangeOfEmployerNotifiedRequest>(viewModel);

            return RedirectToAction(nameof(ChangeOfEmployerNotified), request);
        }

        [HttpGet]
        [Route("overlap-options-with-pending-request")]
        public IActionResult DraftApprenticeshipOverlapOptionsWithPendingRequest(DraftApprenticeshipOverlapOptionWithPendingRequest request)
        {
            var vm = new DraftApprenticeshipOverlapOptionWithPendingRequestViewModel()
            {
                CohortReference = request.CohortReference,
                DraftApprenticeshipHashedId = request.DraftApprenticeshipHashedId,
                DraftApprenticeshipId = request.DraftApprenticeshipId,
                CreatedOn = request.CreatedOn,
                Status = request.Status,
                EnableStopRequestEmail = request.EnableStopRequestEmail
            };

            return View(vm);
        }

        [HttpPost]
        [Route("overlap-options-with-pending-request")]
        public async Task<IActionResult> DraftApprenticeshipOverlapOptionsWithPendingRequest(DraftApprenticeshipOverlapOptionWithPendingRequestViewModel viewModel)
        {
            var mappedViewModel = await _modelMapper.Map<DraftApprenticeshipOverlapOptionViewModel>(viewModel);
            return await OverlapOptionsAction(mappedViewModel);
        }

        [HttpGet]
        [Route("overlap-options")]
        public async Task<IActionResult> DraftApprenticeshipOverlapOptions(DraftApprenticeshipOverlapOptionRequest request)
        {
            var apprenticeshipDetails = await _commitmentsApiClient.GetApprenticeship(request.ApprenticeshipId.Value);

            var enableStopRequestEmail = apprenticeshipDetails.Status == ApprenticeshipStatus.Live
                                        || apprenticeshipDetails.Status == ApprenticeshipStatus.WaitingToStart
                                        || apprenticeshipDetails.Status == ApprenticeshipStatus.Paused
                                        || apprenticeshipDetails.Status == ApprenticeshipStatus.Completed
                                        || apprenticeshipDetails.Status == ApprenticeshipStatus.Stopped;
            if (request.DraftApprenticeshipId.HasValue)
            {
                var pendingOverlapRequests = await _outerApiService.GetOverlapRequest(request.DraftApprenticeshipId.Value);
                if (pendingOverlapRequests.DraftApprenticeshipId.HasValue)
                {
                    return RedirectToAction(nameof(DraftApprenticeshipOverlapOptionsWithPendingRequest), new
                    {
                        ProviderId = apprenticeshipDetails.ProviderId,
                        CohortReference = request.CohortReference,
                        DraftApprenticeshipHashedId = request.DraftApprenticeshipHashedId,
                        CreatedOn = pendingOverlapRequests.CreatedOn,
                        Status = apprenticeshipDetails.Status,
                        EnableStopRequestEmail = enableStopRequestEmail
                    });
                }
            }

            var vm = new DraftApprenticeshipOverlapOptionViewModel
            {
                DraftApprenticeshipHashedId = request.DraftApprenticeshipHashedId,
                Status = apprenticeshipDetails.Status,
                EnableStopRequestEmail = enableStopRequestEmail
            };

            return View(vm);
        }

        [HttpPost]
        [Route("overlap-options")]
        public async Task<IActionResult> DraftApprenticeshipOverlapOptions(DraftApprenticeshipOverlapOptionViewModel viewModel)
        {
            return await OverlapOptionsAction(viewModel);
        }

        private async Task<IActionResult> OverlapOptionsAction(DraftApprenticeshipOverlapOptionViewModel viewModel)
        {
            DraftApprenticeshipViewModel model = string.IsNullOrEmpty(viewModel.DraftApprenticeshipHashedId) ? GetStoredAddDraftApprenticeshipState() : GetStoredEditDraftApprenticeshipState();

            // redirect 302 does not clear tempdata.
            RemoveStoredDraftApprenticeshipState(viewModel.DraftApprenticeshipHashedId);

            if (viewModel.OverlapOptions == OverlapOptions.CompleteActionLater)
            {
                return Redirect(viewModel);
            }

            if (string.IsNullOrEmpty(viewModel.CohortReference))
            {
                await CreateCohortAndDraftApprenticeship(viewModel, model as AddDraftApprenticeshipViewModel);
            }
            else if (string.IsNullOrWhiteSpace(viewModel.DraftApprenticeshipHashedId))
            {
                await AddDraftApprenticeship(viewModel, model as AddDraftApprenticeshipViewModel);
            }
            else
            {
                await UpdateDraftApprenticeship(viewModel.DraftApprenticeshipId.Value, model as EditDraftApprenticeshipViewModel);
            }

            if (viewModel.OverlapOptions == OverlapOptions.SendStopRequest)
            {
                await CreateOverlappingTrainingDateRequest(viewModel);
                return RedirectToAction(nameof(EmployerNotified), new { viewModel.ProviderId, viewModel.CohortReference });
            }

            return RedirectToAction(ControllerConstants.CohortController.Actions.Details, ControllerConstants.CohortController.Name, new { viewModel.ProviderId, viewModel.CohortReference });
        }

        [HttpGet]
        [Route("change-of-employer-notified")]
        public IActionResult ChangeOfEmployerNotified(ChangeOfEmployerNotifiedRequest request)
        {
            var vm = new ChangeOfEmployerNotifiedViewModel { ProviderId = request.ProviderId };
            return View(vm);
        }

        [HttpPost]
        [Route("change-of-employer-notified")]
        public IActionResult ChangeOfEmployerNotified(ChangeOfEmployerNotifiedViewModel vm)
        {
            switch (vm.NextAction)
            {
                case NextAction.ViewAllCohorts:
                    return RedirectToAction("Review", "Cohort", new { vm.ProviderId });
                default:
                    return Redirect(_urlHelper.ProviderApprenticeshipServiceLink("/account"));
            }
        }

        [HttpGet]
        [Route("{cohortReference}/employer-notified")]
        public IActionResult EmployerNotified(EmployerNotifiedRequest request)
        {
            var vm = new EmployerNotifiedViewModel { ProviderId = request.ProviderId, CohortReference = request.CohortReference };
            return View(vm);
        }

        [HttpPost]
        [Route("{cohortReference}/employer-notified")]
        public IActionResult EmployerNotified(EmployerNotifiedViewModel vm)
        {
            switch (vm.NextAction)
            {
                case NextAction.ViewAllCohorts:
                    return RedirectToAction("Review", "Cohort", new { vm.ProviderId });

                case NextAction.AddAnotherApprentice:
                    return RedirectToAction("Details", "Cohort", new { vm.ProviderId, vm.CohortReference });

                case NextAction.ManageApprentices:
                    return RedirectToAction(RouteNames.ApprenticesIndex, "Apprentice", new { vm.ProviderId, vm.CohortReference });

                default:
                    return Redirect(_urlHelper.ProviderApprenticeshipServiceLink("/account"));
            }
        }

        [HttpGet]
        [Route("overlap-alert", Name = RouteNames.DraftApprenticeshipOverlapAlert)]
        public IActionResult DraftApprenticeshipOverlapAlert(DraftApprenticeshipOverlapAlertRequest request)
        {
            DraftApprenticeshipViewModel model = request.DraftApprenticeshipHashedId == null
                ? PeekStoredAddDraftApprenticeshipState()
                : PeekStoredEditDraftApprenticeshipState();

            var vm = new DraftApprenticeshipOverlapAlertViewModel
            {
                DraftApprenticeshipHashedId = request.DraftApprenticeshipHashedId,
                OverlapApprenticeshipHashedId = request.OverlapApprenticeshipHashedId,
                CohortReference = model.CohortReference,
                ProviderId = model.ProviderId,
                StartDate = model.StartDate.Date.GetValueOrDefault(),
                EndDate = model.EndDate.Date.GetValueOrDefault(),
                Uln = model.Uln,
                FirstName = model.FirstName,
                LastName = model.LastName,
                ReservationId = request.ReservationId,
                StartMonthYear = request.StartMonthYear,
                CourseCode = request.CourseCode,
                DeliveryModel = request.DeliveryModel,
                EmployerAccountLegalEntityPublicHashedId = request.EmployerAccountLegalEntityPublicHashedId,
                IsOnFlexiPaymentPilot = model.IsOnFlexiPaymentPilot
            };

            if (request.DraftApprenticeshipHashedId == null && request.CacheKey.HasValue)
            {
                vm.CacheKey = request.CacheKey.Value;
            }

            return View(vm);
        }

        [HttpPost]
        [Route("overlap-alert")]
        public IActionResult DraftApprenticeshipOverlapAlert(DraftApprenticeshipOverlapAlertViewModel viewModel)
        {
            return RedirectToAction(nameof(DraftApprenticeshipOverlapOptions), new DraftApprenticeshipOverlapOptionRequest
            {
                ProviderId = viewModel.ProviderId,
                CohortReference = viewModel.CohortReference,
                DraftApprenticeshipHashedId = viewModel.DraftApprenticeshipHashedId,
                ApprenticeshipHashedId = viewModel.OverlapApprenticeshipHashedId
            });
        }

        private IActionResult Redirect(DraftApprenticeshipOverlapOptionViewModel viewModel)
        {
            if (!string.IsNullOrWhiteSpace(viewModel.CohortReference))
            {
                return RedirectToAction(ControllerConstants.CohortController.Actions.Details, ControllerConstants.CohortController.Name, new { viewModel.ProviderId, viewModel.CohortReference });
            }

            return RedirectToAction(ControllerConstants.CohortController.Actions.Review, ControllerConstants.CohortController.Name, new { viewModel.ProviderId });
        }

        private async Task CreateOverlappingTrainingDateRequest(DraftApprenticeshipOverlapOptionViewModel viewModel)
        {
            var createOverlappingTrainingDateApimRequest = await _modelMapper.Map<CreateOverlappingTrainingDateApimRequest>(viewModel);
            await _outerApiService.CreateOverlappingTrainingDateRequest(createOverlappingTrainingDateApimRequest);
        }

        private async Task UpdateDraftApprenticeship(long draftApprenticeshipId, EditDraftApprenticeshipViewModel editModel)
        {
            var updateRequest = await _modelMapper.Map<UpdateDraftApprenticeshipApimRequest>(editModel);
            updateRequest.IgnoreStartDateOverlap = true;
            await _outerApiService.UpdateDraftApprenticeship(editModel.CohortId.Value, draftApprenticeshipId, updateRequest);
        }

        private async Task AddDraftApprenticeship(DraftApprenticeshipOverlapOptionViewModel viewModel, AddDraftApprenticeshipViewModel model)
        {
            var request = await _modelMapper.Map<AddDraftApprenticeshipApimRequest>(model);
            request.IgnoreStartDateOverlap = true;
            request.UserId = _authenticationService.UserId;

            var response = await _outerApiService.AddDraftApprenticeship(model.CohortId.Value, request);
            viewModel.DraftApprenticeshipId = response.DraftApprenticeshipId;
        }

        private async Task CreateCohortAndDraftApprenticeship(DraftApprenticeshipOverlapOptionViewModel viewModel, AddDraftApprenticeshipViewModel model)
        {
            var request = await _modelMapper.Map<CreateCohortRequest>(model);
            request.IgnoreStartDateOverlap = true;

            var response = await _mediator.Send(request);
            viewModel.CohortReference = response.CohortReference;
            viewModel.DraftApprenticeshipId = response.DraftApprenticeshipId;
        }

        private AddDraftApprenticeshipViewModel GetStoredAddDraftApprenticeshipState()
        {
            return TempData.Get<AddDraftApprenticeshipViewModel>(nameof(AddDraftApprenticeshipViewModel));
        }

        private EditDraftApprenticeshipViewModel GetStoredEditDraftApprenticeshipState()
        {
            return TempData.Get<EditDraftApprenticeshipViewModel>(nameof(EditDraftApprenticeshipViewModel));
        }

        private void RemoveStoredDraftApprenticeshipState(string draftApprenticeshipHashedId)
        {
            TempData.Remove(string.IsNullOrEmpty(draftApprenticeshipHashedId)
                ? nameof(AddDraftApprenticeshipViewModel)
                : nameof(EditDraftApprenticeshipViewModel));
        }

        private AddDraftApprenticeshipViewModel PeekStoredAddDraftApprenticeshipState()
        {
            return TempData.GetButDontRemove<AddDraftApprenticeshipViewModel>(nameof(AddDraftApprenticeshipViewModel));
        }

        private EditDraftApprenticeshipViewModel PeekStoredEditDraftApprenticeshipState()
        {
            return TempData.GetButDontRemove<EditDraftApprenticeshipViewModel>(nameof(EditDraftApprenticeshipViewModel));
        }
    }
}