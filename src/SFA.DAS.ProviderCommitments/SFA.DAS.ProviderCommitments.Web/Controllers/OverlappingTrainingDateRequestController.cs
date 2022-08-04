using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;
using SFA.DAS.ProviderUrlHelper;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}/unapproved/[controller]")]
    public class OverlappingTrainingDateRequestController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IModelMapper _modelMapper;
        private readonly ILinkGenerator _urlHelper;
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly DAS.Authorization.Services.IAuthorizationService _authorizationService;
        private readonly IOuterApiService _outerApiService;
        private IFeatureTogglesService<ProviderFeatureToggle> _featureTogglesService;

        public OverlappingTrainingDateRequestController(IMediator mediator,
            IModelMapper modelMapper,
            ILinkGenerator urlHelper,
            ICommitmentsApiClient commitmentsApiClient,
            SFA.DAS.Authorization.Services.IAuthorizationService authorizationService,
            IOuterApiService outerApiService,
            IFeatureTogglesService<ProviderFeatureToggle> featureTogglesService
            )
        {
            _mediator = mediator;
            _modelMapper = modelMapper;
            _urlHelper = urlHelper;
            _commitmentsApiClient = commitmentsApiClient;
            _authorizationService = authorizationService;
            _outerApiService = outerApiService;
            _featureTogglesService = featureTogglesService;
        }

        [HttpGet]
        [Route("overlap-options")]
        public IActionResult DraftApprenticeshipOverlapOptions(DraftApprenticeshipOverlapOptionRequest request)
        {
            var apprenticeshipDetails = _commitmentsApiClient.GetApprenticeship(request.ApprenticeshipId.Value).Result;

            var featureToggleEnabled = _featureTogglesService.GetFeatureToggle(ProviderFeature.OverlappingTrainingDateWithoutPrefix).IsEnabled;
            var vm = new DraftApprenticeshipOverlapOptionViewModel
            {
                DraftApprenticeshipHashedId = request.DraftApprenticeshipHashedId,
                OverlappingTrainingDateRequestToggleEnabled = featureToggleEnabled,
                Status = apprenticeshipDetails.Status,
                EnableStopRequestEmail = true && (apprenticeshipDetails.Status == CommitmentsV2.Types.ApprenticeshipStatus.Live
                || apprenticeshipDetails.Status == CommitmentsV2.Types.ApprenticeshipStatus.WaitingToStart
                || apprenticeshipDetails.Status == CommitmentsV2.Types.ApprenticeshipStatus.Paused)
            };

            return View(vm);
        }

        [HttpPost]
        [Route("overlap-options")]
        public async Task<IActionResult> DraftApprenticeshipOverlapOptions(DraftApprenticeshipOverlapOptionViewModel viewModel)
        {
            var model = GetStoredAddDraftApprenticeshipState();

            if (viewModel.OverlapOptions == OverlapOptions.AddApprenticeshipLater)
            {
                // redirect 302 does not clear tempdata.
                RemoveStoredDraftApprenticeshipState();
                return Redirect(viewModel);
            }

            if (string.IsNullOrEmpty(viewModel.CohortReference))
            {
                await CreateCohortAndDraftApprenticeship(viewModel, model);
            }
            else if (string.IsNullOrWhiteSpace(viewModel.DraftApprenticeshipHashedId))
            {
                await AddDraftApprenticeship(viewModel, model);
            }
            else
            {
                await UpdateDraftApprenticeship();
            }

            if (viewModel.OverlapOptions == OverlapOptions.SendStopRequest)
            {
                await CreateOverlappingTrainingDateRequest(viewModel);
                return RedirectToAction(nameof(EmployerNotified), new { viewModel.ProviderId, viewModel.CohortReference });
            }

            return RedirectToAction("Details", "Cohort", new { viewModel.ProviderId, viewModel.CohortReference });
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

                default:
                    return Redirect(_urlHelper.ProviderApprenticeshipServiceLink("/account"));
            }
        }

        private IActionResult Redirect(DraftApprenticeshipOverlapOptionViewModel viewModel)
        {
            if (!string.IsNullOrWhiteSpace(viewModel.CohortReference))
            {
                return RedirectToAction("Details", "Cohort", new { viewModel.ProviderId, viewModel.CohortReference });
            }

            return RedirectToAction("Review", "Cohort", new { viewModel.ProviderId });
        }

        private async Task CreateOverlappingTrainingDateRequest(DraftApprenticeshipOverlapOptionViewModel viewModel)
        {
            var featureToggleEnabled = _featureTogglesService.GetFeatureToggle(ProviderFeature.OverlappingTrainingDateWithoutPrefix).IsEnabled;
            if (featureToggleEnabled)
            {
                var createOverlappingTrainingDateApimRequest = await _modelMapper.Map<CreateOverlappingTrainingDateApimRequest>(viewModel);
                await _outerApiService.CreateOverlappingTrainingDateRequest(createOverlappingTrainingDateApimRequest);
            }
            else
            {
                throw new InvalidOperationException("Invalid operation");
            }
        }

        private async Task UpdateDraftApprenticeship()
        {
            var editModel = GetStoredEditDraftApprenticeshipState();
            var updateRequest = await _modelMapper.Map<UpdateDraftApprenticeshipRequest>(editModel);
            updateRequest.IgnoreStartDateOverlap = true;
            await _commitmentsApiClient.UpdateDraftApprenticeship(editModel.CohortId.Value, editModel.DraftApprenticeshipId.Value, updateRequest);
        }

        private async Task AddDraftApprenticeship(DraftApprenticeshipOverlapOptionViewModel viewModel, AddDraftApprenticeshipViewModel model)
        {
            var request = await _modelMapper.Map<AddDraftApprenticeshipRequest>(model);
            request.IgnoreStartDateOverlap = true;
            request.UserId = User.Upn();

            var response = await _commitmentsApiClient.AddDraftApprenticeship(model.CohortId.Value, request);
            viewModel.DraftApprenticeshipId = response.DraftApprenticeshipId;
        }

        private async Task CreateCohortAndDraftApprenticeship(DraftApprenticeshipOverlapOptionViewModel viewModel, AddDraftApprenticeshipViewModel model)
        {
            var request = await _modelMapper.Map<Application.Commands.CreateCohort.CreateCohortRequest>(model);
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

        private void RemoveStoredDraftApprenticeshipState()
        {
            TempData.Remove(nameof(AddDraftApprenticeshipViewModel));
        }
    }
}