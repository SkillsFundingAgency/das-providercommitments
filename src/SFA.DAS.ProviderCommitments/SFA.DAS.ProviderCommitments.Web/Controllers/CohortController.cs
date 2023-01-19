using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.CommitmentPermissions.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Authorization.ProviderPermissions.Options;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Application.Commands.BulkUpload;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Filters;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using SFA.DAS.ProviderUrlHelper;
using CreateCohortRequest = SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort.CreateCohortRequest;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}/unapproved")]
    public class CohortController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IModelMapper _modelMapper;
        private readonly ILinkGenerator _urlHelper;
        private readonly ICommitmentsApiClient _commitmentApiClient;
        private readonly SFA.DAS.Authorization.Services.IAuthorizationService _authorizationService;
        private readonly IEncodingService _encodingService;
        private readonly IOuterApiService _outerApiService;

        public CohortController(IMediator mediator,
            IModelMapper modelMapper,
            ILinkGenerator urlHelper,
            ICommitmentsApiClient commitmentsApiClient,
            SFA.DAS.Authorization.Services.IAuthorizationService authorizationService,
            IEncodingService encodingService,
            IOuterApiService outerApiService
            )
        {
            _mediator = mediator;
            _modelMapper = modelMapper;
            _urlHelper = urlHelper;
            _commitmentApiClient = commitmentsApiClient;
            _authorizationService = authorizationService;
            _encodingService = encodingService;
            _outerApiService = outerApiService;
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
        [Route("apprentices/add")]
        [DasAuthorize(ProviderOperation.CreateCohort)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public IActionResult AddNewDraftApprenticeship(CreateCohortWithDraftApprenticeshipRequest request)
        {
            return RedirectToAction(nameof(SelectCourse), request.CloneBaseValues());
        }

        [HttpGet]
        [Route("choose-cohort", Name = RouteNames.ChooseCohort)]
        public async Task<IActionResult> ChooseCohort(ChooseCohortByProviderRequest request)
        {
            var chooseCohortViewModel = await _modelMapper.Map<ChooseCohortViewModel>(request);
            return View(chooseCohortViewModel);
        }

        [HttpGet]
        [Route("add/select-course")]
        [DasAuthorize(ProviderOperation.CreateCohort)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SelectCourse(CreateCohortWithDraftApprenticeshipRequest request)
        {
            var model = await _modelMapper.Map<SelectCourseViewModel>(request);
            return View("SelectCourse", model);
        }

        [HttpPost]
        [Route("add/select-course")]
        [DasAuthorize(ProviderOperation.CreateCohort)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SelectCourse(SelectCourseViewModel model)
        {
            if (string.IsNullOrEmpty(model.CourseCode))
            {
                throw new CommitmentsApiModelException(new List<ErrorDetail>
                    {new ErrorDetail(nameof(model.CourseCode), "You must select a training course")});
            }

            var request = await _modelMapper.Map<CreateCohortWithDraftApprenticeshipRequest>(model);
            return RedirectToAction(nameof(SelectDeliveryModel), request.CloneBaseValues());
        }

        [HttpGet]
        [Route("add/choose-pilot-status")]
        [DasAuthorize(ProviderOperation.CreateCohort)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> ChoosePilotStatus(CreateCohortWithDraftApprenticeshipRequest request)
        {
            var model = await _modelMapper.Map<ChoosePilotStatusViewModel>(request);
            return View("ChoosePilotStatus", model);
        }

        [HttpPost]
        [Route("add/choose-pilot-status")]
        [DasAuthorize(ProviderOperation.CreateCohort)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> ChoosePilotStatus(ChoosePilotStatusViewModel model)
        {
            if (string.IsNullOrEmpty(model.CourseCode))
            {
                throw new CommitmentsApiModelException(new List<ErrorDetail>
                    {new ErrorDetail(nameof(model.CourseCode), "You must select a training course")});
            }

            var request = await _modelMapper.Map<CreateCohortWithDraftApprenticeshipRequest>(model);
            return RedirectToAction(nameof(SelectDeliveryModel), request.CloneBaseValues());
        }

        [HttpGet]
        [Route("add/select-delivery-model")]
        [DasAuthorize(ProviderOperation.CreateCohort)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SelectDeliveryModel(CreateCohortWithDraftApprenticeshipRequest request)
        {
            var model = await _modelMapper.Map<SelectDeliveryModelViewModel>(request);

            if (model.DeliveryModels.Length > 1)
            {
                return View("SelectDeliveryModel", model);
            }

            request.DeliveryModel = model.DeliveryModels.FirstOrDefault();
            return RedirectToAction(nameof(AddDraftApprenticeship), request.CloneBaseValues());
        }

        [HttpPost]
        [Route("add/select-delivery-model")]
        [DasAuthorize(ProviderOperation.CreateCohort)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SetDeliveryModel(SelectDeliveryModelViewModel model)
        {
            if (model.DeliveryModel == null)
            {
                throw new CommitmentsApiModelException(new List<ErrorDetail>
                    {new ErrorDetail("DeliveryModel", "You must select the apprenticeship delivery model")});
            }

            var request = await _modelMapper.Map<CreateCohortWithDraftApprenticeshipRequest>(model);
            return RedirectToAction(nameof(AddDraftApprenticeship), request.CloneBaseValues());
        }

        [HttpGet]
        [Route("add/apprenticeship")]
        [DasAuthorize(ProviderOperation.CreateCohort)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> AddDraftApprenticeship(CreateCohortWithDraftApprenticeshipRequest request)
        {
            var model = GetStoredDraftApprenticeshipState();
            if (model == null)
            {
                model = await _modelMapper.Map<AddDraftApprenticeshipViewModel>(request);
            }
            else
            {
                model.CourseCode = request.CourseCode;
                model.DeliveryModel = request.DeliveryModel;
            }
            return View("AddDraftApprenticeship", model);
        }


        [HttpPost]
        [Route("add/validate")]
        [AjaxValidation]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> Validate(Models.AddDraftApprenticeshipViewModel model)
        {
            var request = await _modelMapper.Map<ValidateDraftApprenticeshipApimRequest>(model);
            request.UserId = User.Upn();
            try
            {
                await _outerApiService.ValidateDraftApprenticeshipForOverlappingTrainingDateRequest(request);
            }
            catch (CommitmentsApiModelException ex)
            {
                return Json(ex.Errors);
            }
            return new OkResult();
        }

        [HttpPost]
        [Route("add/apprenticeship", Name = RouteNames.CohortAddApprenticeship)]
        [DasAuthorize(ProviderOperation.CreateCohort)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> AddDraftApprenticeshipOrRoute(string changeCourse, string changeDeliveryModel, AddDraftApprenticeshipViewModel model)
        {
            if (changeCourse == "Edit" || changeDeliveryModel == "Edit")
            {
                StoreDraftApprenticeshipState(model);
                var request = await _modelMapper.Map<CreateCohortWithDraftApprenticeshipRequest>(model);
                request.ShowTrainingDetails = true;
                return RedirectToAction(changeCourse == "Edit" ? nameof(SelectCourse) : nameof(SelectDeliveryModel), request.CloneBaseValues());
            }

            var overlapResult = await HasStartDateOverlap(model);
            if (overlapResult != null && overlapResult.HasStartDateOverlap && overlapResult.HasOverlapWithApprenticeshipId.HasValue)
            {
                StoreDraftApprenticeshipState(model);
                var hashedApprenticeshipId = _encodingService.Encode(overlapResult.HasOverlapWithApprenticeshipId.Value, EncodingType.ApprenticeshipId);
                return RedirectToAction("DraftApprenticeshipOverlapAlert", "OverlappingTrainingDateRequest", new
                {
                    OverlapApprenticeshipHashedId = hashedApprenticeshipId,
                    ReservationId = model.ReservationId,
                    StartMonthYear = model.StartDate.MonthYear,
                    CourseCode = model.CourseCode,
                    DeliveryModel = model.DeliveryModel,
                    EmployerAccountLegalEntityPublicHashedId = model.EmployerAccountLegalEntityPublicHashedId
                });
            }

            return await SaveDraftApprenticeship(model);
        }

        private async Task<IActionResult> SaveDraftApprenticeship(AddDraftApprenticeshipViewModel model)
        {
            var request = await _modelMapper.Map<CreateCohortRequest>(model);

            var response = await _mediator.Send(request);

            if (RequireRpl(model.StartDate))
            {
                var draftApprenticeshipHashedId = _encodingService.Encode(response.DraftApprenticeshipId.Value, EncodingType.ApprenticeshipId);
                return RedirectToAction("RecognisePriorLearning", "DraftApprenticeship", new { response.CohortReference, draftApprenticeshipHashedId });
            }
            else if (response.HasStandardOptions)
            {
                var draftApprenticeshipHashedId = _encodingService.Encode(response.DraftApprenticeshipId.Value, EncodingType.ApprenticeshipId);
                return RedirectToAction("SelectOptions", "DraftApprenticeship", new { model.ProviderId, DraftApprenticeshipHashedId = draftApprenticeshipHashedId, response.CohortReference });
            }

            return RedirectToAction(nameof(Details), new { model.ProviderId, response.CohortReference });
        }

        private bool RequireRpl(MonthYearModel startDate)
            => startDate?.Date >= new DateTime(2022, 08, 01);
        
        [HttpGet]
        [Route("add/select-employer", Name = RouteNames.NewCohortSelectEmployer)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SelectEmployer(SelectEmployerRequest request)
        {
            var model = await _modelMapper.Map<SelectEmployerViewModel>(request);
            return View(model);
        }

        [HttpGet]
        [Route("add/confirm-employer")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> ConfirmEmployer(ConfirmEmployerRequest request)
        {
            var model = await _modelMapper.Map<ConfirmEmployerViewModel>(request);

            return View(model);
        }

        [HttpPost]
        [Route("add/confirm-employer")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public IActionResult ConfirmEmployer(ConfirmEmployerViewModel viewModel)
        {
            if (viewModel.Confirm.Value)
            {
                //return Redirect($"https://at-permissions-api.apprenticeships.education.gov.uk/{viewModel.ProviderId}/reservations/{viewModel.EmployerAccountLegalEntityPublicHashedId}/select");
                var url = _urlHelper.ReservationsLink($"{viewModel.ProviderId}/reservations/{viewModel.EmployerAccountLegalEntityPublicHashedId}/select");
                return Redirect(url);
            }

            return RedirectToAction("SelectEmployer", new { viewModel.ProviderId });
        }

        [HttpGet]
        [Route("{cohortReference}/details/delete")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> Delete(DeleteCohortRequest request)
        {
            var model = await _modelMapper.Map<DeleteCohortViewModel>(request);

            return View(model);
        }

        [HttpPost]
        [Route("{cohortReference}/details/delete")]
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

        [Route("{cohortReference}")]
        [Route("{cohortReference}/details")]
        [DasAuthorize(CommitmentOperation.AccessCohort)]
        [Authorize(Policy = nameof(PolicyNames.HasViewerOrAbovePermission))]
        public async Task<IActionResult> Details(DetailsRequest request)
        {
            var viewModel = await _modelMapper.Map<DetailsViewModel>(request);
            return View(viewModel);
        }

        [Route("{cohortReference}")]
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
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public IActionResult FileUploadInform(SelectAddDraftApprenticeshipJourneyRequest request)
        {
            var model = new FileUploadStartViewModel { ProviderId = request.ProviderId };
            return View(model);
        }

        [HttpGet]
        [Route("add/file-upload/start")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public IActionResult FileUploadStart(SelectAddDraftApprenticeshipJourneyRequest request)
        {
            var model = new FileUploadStartViewModel { ProviderId = request.ProviderId };
            return View(model);
        }

        [HttpPost]
        [Route("add/file-upload/start")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        [ServiceFilter(typeof(HandleBulkUploadValidationErrorsAttribute))]
        public async Task<IActionResult> FileUploadStart(FileUploadStartViewModel viewModel)
        {
            await ValidateBulkUploadData(viewModel.ProviderId, viewModel.Attachment);
            var request = await _modelMapper.Map<FileUploadReviewRequest>(viewModel);
            return RedirectToAction(nameof(FileUploadReview), request);
        }

        [HttpGet]
        [Route("add/file-upload/validate")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> FileUploadValidationErrors(FileUploadValidateErrorRequest request)
        {
            var viewModel = await _modelMapper.Map<FileUploadValidateViewModel>(request);
            return View(viewModel);
        }

        [HttpPost]
        [Route("add/file-upload/validate")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        [ServiceFilter(typeof(HandleBulkUploadValidationErrorsAttribute))]
        public async Task<IActionResult> FileUploadValidationErrors(FileUploadValidateViewModel viewModel)
        {
            await ValidateBulkUploadData(viewModel.ProviderId, viewModel.Attachment);
            var request = await _modelMapper.Map<FileUploadReviewRequest>(viewModel);
            return RedirectToAction(nameof(FileUploadReview), request);
        }

        [HttpGet]
        [Route("add/file-upload/review")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> FileUploadReview(FileUploadReviewRequest request)
        {
            var viewModel = await _modelMapper.Map<FileUploadReviewViewModel>(request);
            return View(viewModel);
        }

        [HttpPost]
        [Route("add/file-upload/review")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        [ServiceFilter(typeof(HandleBulkUploadValidationErrorsAttribute))]
        public async Task<IActionResult> FileUploadReview(FileUploadReviewViewModel viewModel)
        {
            if (viewModel.SelectedOption == FileUploadReviewOption.ApproveAndSend)
            {
                var approveApiRequest = await _modelMapper.Map<Infrastructure.OuterApi.Requests.BulkUploadAddAndApproveDraftApprenticeshipsRequest>(viewModel);
                var approvedResponse = await _outerApiService.BulkUploadAddAndApproveDraftApprenticeships(approveApiRequest);
                TempData.Put(Constants.BulkUpload.ApprovedApprenticeshipResponse, approvedResponse);
                return RedirectToAction(nameof(FileUploadSuccess), viewModel.ProviderId);
            }

            switch (viewModel.SelectedOption)
            {
                // TODO re-add this route when the Add/Approve feature is turned back on
                //case FileUploadReviewOption.ApproveAndSend:
                //    var approveApiRequest = await _modelMapper.Map<Infrastructure.OuterApi.Requests.BulkUploadAddAndApproveDraftApprenticeshipsRequest>(viewModel);
                //    var approvedResponse = await _outerApiService.BulkUploadAddAndApproveDraftApprenticeships(approveApiRequest);
                //    TempData.Put(Constants.BulkUpload.ApprovedApprenticeshipResponse, approvedResponse);
                //    return RedirectToAction(nameof(FileUploadSuccess), viewModel.ProviderId);

                case FileUploadReviewOption.SaveButDontSend:
                    var apiRequest = await _modelMapper.Map<Infrastructure.OuterApi.Requests.BulkUploadAddDraftApprenticeshipsRequest>(viewModel);
                    var response = await _outerApiService.BulkUploadDraftApprenticeships(apiRequest);
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
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public IActionResult FileUploadDiscard(FileDiscardRequest fileDiscardRequest)
        {
            var viewModel = new FileDiscardViewModel { CacheRequestId = fileDiscardRequest.CacheRequestId, ProviderId = fileDiscardRequest.ProviderId };
            return View(viewModel);
        }

        [HttpPost]
        [Route("add/file-upload/discard-file")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public IActionResult FileUploadDiscard(FileDiscardViewModel viewModel)
        {
            if (viewModel.FileDiscardConfirmed != null && (bool)viewModel.FileDiscardConfirmed)
            {
                return RedirectToAction(nameof(FileUploadReviewDelete), new FileUploadReviewDeleteRequest { ProviderId = viewModel.ProviderId, CacheRequestId = viewModel.CacheRequestId, RedirectTo = FileUploadReviewDeleteRedirect.SuccessDiscardFile });
            }

            return RedirectToAction(nameof(FileUploadReview), new { ProviderId = viewModel.ProviderId, CacheRequestId = viewModel.CacheRequestId });
        }

        [HttpGet]
        [Route("add/file-upload/review-delete")]
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
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> FileUploadAmendedFile(FileUploadAmendedFileRequest request)
        {
            var viewModel = await _modelMapper.Map<FileUploadAmendedFileViewModel>(request);
            return View(viewModel);
        }

        [HttpPost]
        [Route("add/file-upload/amended-file")]
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
        public async Task<IActionResult> SelectAddDraftApprenticeshipJourney(SelectAddDraftApprenticeshipJourneyRequest request)
        {
            var model = await _modelMapper.Map<SelectAddDraftApprenticeshipJourneyViewModel>(request);
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

        private void StoreDraftApprenticeshipState(AddDraftApprenticeshipViewModel model)
        {
            TempData.Put(nameof(AddDraftApprenticeshipViewModel), model);
        }

        private AddDraftApprenticeshipViewModel GetStoredDraftApprenticeshipState()
        {
            return TempData.Get<AddDraftApprenticeshipViewModel>(nameof(AddDraftApprenticeshipViewModel));
        }

        private async Task ValidateBulkUploadData(long providerId, IFormFile attachment)
        {
            var bulkValidate = new FileUploadValidateDataRequest { Attachment = attachment, ProviderId = providerId };
            await _mediator.Send(bulkValidate);
        }

        private async Task<Infrastructure.OuterApi.Responses.ValidateUlnOverlapOnStartDateQueryResult> HasStartDateOverlap(AddDraftApprenticeshipViewModel model)
        {
            if (model.StartDate.Date.HasValue && model.EndDate.Date.HasValue && !string.IsNullOrWhiteSpace(model.Uln))
            {
                var apimRequest = await _modelMapper.Map<ValidateDraftApprenticeshipApimRequest>(model);
                await _outerApiService.ValidateDraftApprenticeshipForOverlappingTrainingDateRequest(apimRequest);

                var result = await _outerApiService.ValidateUlnOverlapOnStartDate(
                model.ProviderId,
                model.Uln,
                model.StartDate.Date.Value.ToString("dd-MM-yyyy"),
                model.EndDate.Date.Value.ToString("dd-MM-yyyy")
                );

                return result;
            }

            return null;
        }
    }
}