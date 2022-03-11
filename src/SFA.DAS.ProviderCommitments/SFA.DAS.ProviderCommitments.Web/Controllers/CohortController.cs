using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFA.DAS.Authorization.CommitmentPermissions.Options;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.Authorization.ProviderPermissions.Options;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Application.Commands.BulkUpload;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using SFA.DAS.ProviderUrlHelper;
using CreateCohortRequest = SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort.CreateCohortRequest;
using System.Linq;
using System.Collections.Generic;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}/unapproved")]
    public class CohortController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IModelMapper _modelMapper;
        private readonly ILinkGenerator _urlHelper;
        private readonly ICommitmentsApiClient _commitmentApiClient;
        private readonly IFeatureTogglesService<ProviderFeatureToggle> _featureTogglesService;
        private readonly IEncodingService _encodingService;

        public CohortController(IMediator mediator,
            IModelMapper modelMapper,
            ILinkGenerator urlHelper,
            ICommitmentsApiClient commitmentsApiClient,
            IFeatureTogglesService<ProviderFeatureToggle> featureTogglesService,
            IEncodingService encodingService)
        {
            _mediator = mediator;
            _modelMapper = modelMapper;
            _urlHelper = urlHelper;
            _commitmentApiClient = commitmentsApiClient;
            _featureTogglesService = featureTogglesService;
            _encodingService = encodingService;
        }

        [HttpGet]
        [Route("review", Name = RouteNames.CohortReview)]
        [Route("", Name = RouteNames.Cohort)]
        public async Task<IActionResult> Review(CohortsByProviderRequest request)
        {
            var reviewViewModel = await _modelMapper.Map<ReviewViewModel>(request);
            reviewViewModel.SortedByHeader();

            return View(reviewViewModel);
        }

        [HttpGet]
        [Route("draft", Name = RouteNames.CohortDraft)]
        public async Task<IActionResult> Draft(CohortsByProviderRequest request)
        {
            var draftViewModel = await _modelMapper.Map<DraftViewModel>(request);
            draftViewModel.SortedByHeader();

            return View(draftViewModel);
        }

        [HttpGet]
        [Route("with-employer", Name = RouteNames.CohortWithEmployer)]
        public async Task<IActionResult> WithEmployer(CohortsByProviderRequest request)
        {
            var withEmployerViewModel = await _modelMapper.Map<WithEmployerViewModel>(request);
            withEmployerViewModel.SortedByHeader();

            return View(withEmployerViewModel);
        }

        [HttpGet]
        [Route("with-transfer-sender", Name = RouteNames.CohortWithTransferSender)]
        public async Task<IActionResult> WithTransferSender(CohortsByProviderRequest request)
        {
            var withTransferSenderViewModel = await _modelMapper.Map<WithTransferSenderViewModel>(request);
            withTransferSenderViewModel.SortedByHeader();

            return View(withTransferSenderViewModel);
        }

        [HttpGet]
        [Route("add-apprentice")]
        [Route("add/apprentice")]
        [DasAuthorize(ProviderOperation.CreateCohort)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> AddDraftApprenticeship(CreateCohortWithDraftApprenticeshipRequest request)
        {
            var model = await _modelMapper.Map<AddDraftApprenticeshipViewModel>(request);
            return View(model);
        }

        [HttpGet]
        [Route("choose-cohort", Name = RouteNames.ChooseCohort)]
        public async Task<IActionResult> ChooseCohort(ChooseCohortByProviderRequest request)
        {
            var chooseCohortViewModel = await _modelMapper.Map<ChooseCohortViewModel>(request);
            return View(chooseCohortViewModel);
        }

        [HttpPost]
        [Route("add-apprentice")]
        [Route("add/apprentice")]
        [DasAuthorize(ProviderOperation.CreateCohort)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> AddDraftApprenticeship(AddDraftApprenticeshipViewModel model)
        {
            var request = await _modelMapper.Map<CreateCohortRequest>(model);

            var response = await _mediator.Send(request);

            if (response.DraftApprenticeshipId.HasValue)
            {
                var draftApprenticeshipHashedId = _encodingService.Encode(response.DraftApprenticeshipId.Value,
                    EncodingType.ApprenticeshipId);
                return RedirectToAction("SelectOptions", "DraftApprenticeship", new { model.ProviderId, DraftApprenticeshipHashedId = draftApprenticeshipHashedId, response.CohortReference });
            }

            return RedirectToAction(nameof(Details), new { model.ProviderId, response.CohortReference });
        }

        [HttpGet]
        [Route("add/select-employer", Name = RouteNames.NewCohortSelectEmployer)]
        [DasAuthorize(ProviderFeature.ProviderCreateCohortV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SelectEmployer(SelectEmployerRequest request)
        {
            var model = await _modelMapper.Map<SelectEmployerViewModel>(request);
            return View(model);
        }

        [HttpGet]
        [Route("add/confirm-employer")]
        [DasAuthorize(ProviderFeature.ProviderCreateCohortV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> ConfirmEmployer(ConfirmEmployerRequest request)
        {
            var model = await _modelMapper.Map<ConfirmEmployerViewModel>(request);

            return View(model);
        }

        [HttpPost]
        [Route("add/confirm-employer")]
        [DasAuthorize(ProviderFeature.ProviderCreateCohortV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public IActionResult ConfirmEmployer(ConfirmEmployerViewModel viewModel)
        {
            if (viewModel.Confirm.Value)
            {
                return Redirect(_urlHelper.ReservationsLink($"{viewModel.ProviderId}/reservations/{viewModel.EmployerAccountLegalEntityPublicHashedId}/select"));
            }

            return RedirectToAction("SelectEmployer", new { viewModel.ProviderId });
        }

        [HttpGet]
        [Route("{cohortReference}/details/delete")]
        [DasAuthorize(ProviderFeature.ProviderCreateCohortV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> Delete(DeleteCohortRequest request)
        {
            var model = await _modelMapper.Map<DeleteCohortViewModel>(request);

            return View(model);
        }

        [HttpPost]
        [Route("{cohortReference}/details/delete")]
        [DasAuthorize(ProviderFeature.ProviderCreateCohortV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> Delete([FromServices] IAuthenticationService authenticationService, DeleteCohortViewModel viewModel)
        {
            if (viewModel.Confirm.Value)
            {
                CommitmentsV2.Types.UserInfo userInfo = authenticationService.UserInfo;
                await _commitmentApiClient.DeleteCohort(viewModel.CohortId, userInfo);
                return RedirectToAction("Review", new { viewModel.ProviderId });
            }

            return RedirectToAction(nameof(Details), new { viewModel.ProviderId, viewModel.CohortReference });
        }

        [Route("{cohortReference}/details")]
        [DasAuthorize(CommitmentOperation.AccessCohort)]
        [Authorize(Policy = nameof(PolicyNames.HasViewerOrAbovePermission))]
        public async Task<IActionResult> Details(DetailsRequest request)
        {
            var viewModel = await _modelMapper.Map<DetailsViewModel>(request);
            return View(viewModel);
        }

        [Route("{cohortReference}/details")]
        [DasAuthorize(CommitmentOperation.AccessCohort)]
        [Authorize(Policy = nameof(PolicyNames.HasViewerOrAbovePermission))]
        [HttpPost]
        public async Task<IActionResult> Details([FromServices] IPolicyAuthorizationWrapper authorizationService, DetailsViewModel viewModel)
        {
            switch (viewModel.Selection)
            {
                case CohortDetailsOptions.Send:
                    {
                        await ValidateAuthorization(authorizationService);
                        var request = await _modelMapper.Map<SendCohortRequest>(viewModel);
                        await _commitmentApiClient.SendCohort(viewModel.CohortId, request);
                        return RedirectToAction(nameof(Acknowledgement), new { viewModel.CohortReference, viewModel.ProviderId, SaveStatus = SaveStatus.AmendAndSend });
                    }
                case CohortDetailsOptions.Approve:
                    {
                        await ValidateAuthorization(authorizationService);
                        var request = await _modelMapper.Map<ApproveCohortRequest>(viewModel);
                        await _commitmentApiClient.ApproveCohort(viewModel.CohortId, request);
                        var saveStatus = viewModel.IsApprovedByEmployer && string.IsNullOrEmpty(viewModel.TransferSenderHashedId) ? SaveStatus.Approve : SaveStatus.ApproveAndSend;
                        return RedirectToAction(nameof(Acknowledgement), new { viewModel.CohortReference, viewModel.ProviderId, SaveStatus = saveStatus });
                    }
                case CohortDetailsOptions.ApprenticeRequest:
                    {
                        return RedirectToAction("Review", new { viewModel.ProviderId });
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(viewModel.Selection));
            }
        }

        [HttpGet]
        [Route("{cohortReference}/Acknowledgement")]
        [DasAuthorize(CommitmentOperation.AccessCohort)]
        public async Task<ActionResult> Acknowledgement(AcknowledgementRequest request)
        {
            var model = await _modelMapper.Map<AcknowledgementViewModel>(request);
            return View(model);
        }

        [HttpGet]
        [Route("add/entry-method")]
        [DasAuthorize(ProviderFeature.BulkUploadV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public IActionResult SelectDraftApprenticeshipsEntryMethod(SelectAddDraftApprenticeshipJourneyRequest request)
        {
            var model = new SelectAddDraftApprenticeshipJourneyViewModel { ProviderId = request.ProviderId };
            return View(model);
        }

        [HttpPost]
        [Route("add/entry-method")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public IActionResult SelectDraftApprenticeshipsEntryMethod(SelectDraftApprenticeshipsEntryMethodViewModel viewModel)
        {
            if (viewModel.Selection == AddDraftApprenticeshipEntryMethodOptions.BulkCsv)
            {
                return RedirectToAction(nameof(FileUploadInform), new { ProviderId = viewModel.ProviderId });
            }
            else if (viewModel.Selection == AddDraftApprenticeshipEntryMethodOptions.Manual)
            {
                return RedirectToAction(nameof(SelectAddDraftApprenticeshipJourney), new { ProviderId = viewModel.ProviderId });
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        [HttpGet]
        [Route("add/file-upload/inform")]
        [DasAuthorize(ProviderFeature.BulkUploadV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public IActionResult FileUploadInform(SelectAddDraftApprenticeshipJourneyRequest request)
        {
            var model = new FileUploadStartViewModel { ProviderId = request.ProviderId };
            return View(model);
        }

        [HttpGet]
        [Route("add/file-upload/start")]
        [DasAuthorize(ProviderFeature.BulkUploadV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public IActionResult FileUploadStart(SelectAddDraftApprenticeshipJourneyRequest request)
        {
            var model = new FileUploadStartViewModel { ProviderId = request.ProviderId };
            return View(model);
        }

        [HttpPost]
        [Route("add/file-upload/start")]
        [DasAuthorize(ProviderFeature.BulkUploadV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> FileUploadStart(FileUploadStartViewModel viewModel)
        {
            var hasErrors = await ValidateBulkUploadData(viewModel.ProviderId, viewModel.Attachment);
            if (hasErrors)
            {
                return RedirectToAction(nameof(FileUploadValidationErrors), new FileUploadValidateErrorRequest { ProviderId = viewModel.ProviderId });
            }
            else
            {
                var request = await _modelMapper.Map<FileUploadReviewRequest>(viewModel);
                return RedirectToAction(nameof(FileUploadReview), request);
            }
        }

        private async Task<bool> ValidateBulkUploadData(long providerId, IFormFile attachment)
        {
            var bulkValidate = new FileUploadValidateDataRequest { Attachment = attachment, ProviderId = providerId };
            var response = await _mediator.Send(bulkValidate);
            if (response.BulkUploadValidationErrors != null && response.BulkUploadValidationErrors.Count > 0)
            {
                TempData.Put(Constants.BulkUpload.BulkUploadErrors, response);
                return true;
            }

            return false;
        }

        [HttpGet]
        [Route("add/file-upload/validate")]
        [DasAuthorize(ProviderFeature.BulkUploadV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> FileUploadValidationErrors(FileUploadValidateErrorRequest request)
        {
            var errors = TempData.Get<BulkUploadValidateApiResponse>(Constants.BulkUpload.BulkUploadErrors);
            if (errors == null)
            {
                return RedirectToAction(nameof(FileUploadStart), new SelectAddDraftApprenticeshipJourneyRequest { ProviderId = request.ProviderId });
            }

            var viewModel = await _modelMapper.Map<FileUploadValidateViewModel>(errors);
            viewModel.ProviderId = request.ProviderId;
            return View(viewModel);
        }

        [HttpGet]
        [Route("add/file-upload/review")]
        [DasAuthorize(ProviderFeature.BulkUploadV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> FileUploadReview(FileUploadReviewRequest request)
        {
            var viewModel = await _modelMapper.Map<FileUploadReviewViewModel>(request);
            return View(viewModel);
        }

        [HttpPost]
        [Route("add/file-upload/review")]
        [DasAuthorize(ProviderFeature.BulkUploadV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> FileUploadReview(FileUploadReviewViewModel viewModel)
        {
            switch (viewModel.SelectedOption)
            {
                case FileUploadReviewOption.ApproveAndSend:
                    var approvedApiRequest = await _modelMapper.Map<BulkUploadAddAndApproveDraftApprenticeshipsRequest>(viewModel);
                    var approvedResponse = await _commitmentApiClient.BulkUploadAddAndApproveDraftApprenticeships(viewModel.ProviderId, approvedApiRequest);
                    TempData.Put(Constants.BulkUpload.ApprovedApprenticeshipResponse, approvedResponse);
                    return RedirectToAction(nameof(FileUploadSuccess), viewModel.ProviderId);
                case FileUploadReviewOption.SaveButDontSend:
                    var apiRequest = await _modelMapper.Map<BulkUploadAddDraftApprenticeshipsRequest>(viewModel);
                    var response = await _commitmentApiClient.BulkUploadDraftApprenticeships(viewModel.ProviderId, apiRequest);
                    TempData.Put(Constants.BulkUpload.DraftApprenticeshipResponse, response);
                    return RedirectToAction(nameof(FileUploadSuccessSaveDraft), viewModel.ProviderId);                    
                default:
                    return RedirectToAction(nameof(FileUploadAmendedFile), new FileUploadAmendedFileRequest { ProviderId = viewModel.ProviderId, CacheRequestId = viewModel.CacheRequestId });
            }
        }

        [HttpGet]
        [Route("add/file-upload/success-save-draft", Name = RouteNames.SuccessSaveDraft)]
        public async Task<IActionResult> FileUploadSuccessSaveDraft(long providerId)
        {
            var response = TempData.GetButDontRemove<GetBulkUploadAddDraftApprenticeshipsResponse>(Constants.BulkUpload.DraftApprenticeshipResponse);
            var viewModel = await _modelMapper.Map<BulkUploadAddDraftApprenticeshipsViewModel>(response);
            viewModel.ProviderId = providerId;
            return View(viewModel);
        }

        [HttpGet]
        [Route("add/file-upload/success", Name = RouteNames.SuccessSendToEmployer)]
        public async Task<IActionResult> FileUploadSuccess(long providerId)
        {
            var response = TempData.GetButDontRemove<BulkUploadAddAndApproveDraftApprenticeshipsResponse>(Constants.BulkUpload.ApprovedApprenticeshipResponse);
            var viewModel = await _modelMapper.Map<BulkUploadAddAndApproveDraftApprenticeshipsViewModel>(response);
            viewModel.ProviderId = providerId;
            return View(viewModel);
        }

        [HttpGet]
        [Route("add/file-upload/discard-file")]
        [DasAuthorize(ProviderFeature.BulkUploadV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public IActionResult FileUploadDiscard(FileDiscardRequest fileDiscardRequest)
        {
            var viewModel = new FileDiscardViewModel { CacheRequestId = fileDiscardRequest.CacheRequestId, ProviderId = fileDiscardRequest.ProviderId };
            return View(viewModel);
        }

        [HttpPost]
        [Route("add/file-upload/discard-file")]
        [DasAuthorize(ProviderFeature.BulkUploadV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public IActionResult FileUploadDiscard(FileDiscardViewModel viewModel)
        {         
            if (viewModel.FileDiscardConfirmed != null &&  (bool)viewModel.FileDiscardConfirmed)
            {
                return RedirectToAction(nameof(FileUploadReviewDelete), new FileUploadReviewDeleteRequest { ProviderId = viewModel.ProviderId, CacheRequestId = viewModel.CacheRequestId, RedirectTo = FileUploadReviewDeleteRedirect.SuccessDiscardFile });
            }

            return RedirectToAction(nameof(FileUploadReview), new { ProviderId = viewModel.ProviderId, CacheRequestId = viewModel.CacheRequestId });
        }


        [HttpGet]
        [Route("add/file-upload/review-delete")]
        [DasAuthorize(ProviderFeature.BulkUploadV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> FileUploadReviewDelete(FileUploadReviewDeleteRequest deleteRequest)
        {
            await _mediator.Send(new DeleteCachedFileCommand { CachedRequestId = deleteRequest.CacheRequestId });
            if (deleteRequest.RedirectTo.HasValue)
            {
                if (deleteRequest.RedirectTo.Value == FileUploadReviewDeleteRedirect.Home)
                {
                    return Redirect(_urlHelper.ProviderApprenticeshipServiceLink("/account"));
                }

                if (deleteRequest.RedirectTo.Value == FileUploadReviewDeleteRedirect.SuccessDiscardFile)
                {
                    var viewModel = new FileDiscardSuccessViewModel { ProviderId = deleteRequest.ProviderId };
                    return View("FileDiscardSuccess", viewModel);
                }
            }

            return RedirectToAction(nameof(FileUploadStart), new { ProviderId = deleteRequest.ProviderId });
        }

        [HttpGet]
        [Route("add/file-upload/amended-file")]
        [DasAuthorize(ProviderFeature.BulkUploadV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> FileUploadAmendedFile(FileUploadAmendedFileRequest request)
        {
            var viewModel = await _modelMapper.Map<FileUploadAmendedFileViewModel>(request);
            return View(viewModel);
        }

        [HttpPost]
        [Route("add/file-upload/amended-file")]
        [DasAuthorize(ProviderFeature.BulkUploadV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> FileUploadAmendedFile(FileUploadAmendedFileViewModel viewModel)
        {
            if (viewModel.Confirm.Value)
            {
                await _mediator.Send(new DeleteCachedFileCommand { CachedRequestId = viewModel.CacheRequestId });
                return RedirectToAction(nameof(FileUploadStart), new SelectAddDraftApprenticeshipJourneyRequest { ProviderId = viewModel.ProviderId });
            }

            return RedirectToAction(nameof(FileUploadReview), new FileUploadReviewRequest { CacheRequestId = viewModel.CacheRequestId, ProviderId = viewModel.ProviderId });
        }

        [HttpGet]
        [Route("add/select-journey")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public IActionResult SelectAddDraftApprenticeshipJourney(SelectAddDraftApprenticeshipJourneyRequest request)
        {
            var model = new SelectAddDraftApprenticeshipJourneyViewModel
            {
                ProviderId = request.ProviderId,
                IsBulkUploadV2Enabled = _featureTogglesService.GetFeatureToggle(ProviderFeature.BulkUploadV2WithoutPrefix).IsEnabled
            };

            return View(model);
        }

        [HttpPost]
        [Route("add/select-journey")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public IActionResult SelectAddDraftApprenticeshipJourney(SelectAddDraftApprenticeshipJourneyViewModel viewModel)
        {
            if (viewModel.Selection == AddDraftApprenticeshipJourneyOptions.ExistingCohort)
            {
                return RedirectToAction(nameof(ChooseCohort), new { ProviderId = viewModel.ProviderId });
            }
            else if (viewModel.Selection == AddDraftApprenticeshipJourneyOptions.NewCohort)
            {
                return RedirectToAction(nameof(SelectEmployer), new { ProviderId = viewModel.ProviderId });
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        [HttpGet]
        [Route("add/file-upload/review-cohort")]
        [DasAuthorize(ProviderFeature.BulkUploadV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> FileUploadReviewApprentices(FileUploadReviewApprenticeRequest reviewApprenticeRequest)
        {   
            var viewModel = await _modelMapper.Map<FileUploadReviewApprenticeViewModel>(reviewApprenticeRequest);
            return View(viewModel);
        }

        private async Task ValidateAuthorization(IPolicyAuthorizationWrapper authorizationService)
        {
            var result = await authorizationService.IsAuthorized(User, PolicyNames.HasContributorWithApprovalOrAbovePermission);

            if (!result)
            {
                throw new UnauthorizedAccessException("User not allowed");
            }
        }
    }
}