using Microsoft.AspNetCore.Authorization;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Application.Commands.BulkUpload;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Exceptions;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Filters;
using SFA.DAS.ProviderCommitments.Web.Helpers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using SFA.DAS.ProviderUrlHelper;
using IAuthorizationService = SFA.DAS.ProviderCommitments.Interfaces.IAuthorizationService;
using SelectCourseViewModel = SFA.DAS.ProviderCommitments.Web.Models.Cohort.SelectCourseViewModel;
using SelectDeliveryModelViewModel = SFA.DAS.ProviderCommitments.Web.Models.Cohort.SelectDeliveryModelViewModel;

namespace SFA.DAS.ProviderCommitments.Web.Controllers;

[Route("{providerId}/unapproved")]
public class CohortController : Controller
{
    private readonly IMediator _mediator;
    private readonly IModelMapper _modelMapper;
    private readonly ILinkGenerator _urlHelper;
    private readonly ICommitmentsApiClient _commitmentApiClient;
    private readonly IEncodingService _encodingService;
    private readonly IOuterApiService _outerApiService;
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<CohortController> _logger;

    public CohortController(IMediator mediator,
        IModelMapper modelMapper,
        ILinkGenerator urlHelper,
        ICommitmentsApiClient commitmentsApiClient,
        IEncodingService encodingService,
        IOuterApiService outerApiService,
        IAuthorizationService authorizationService, 
        ILogger<CohortController> logger)
    {
        _mediator = mediator;
        _modelMapper = modelMapper;
        _urlHelper = urlHelper;
        _commitmentApiClient = commitmentsApiClient;
        _encodingService = encodingService;
        _outerApiService = outerApiService;
        _authorizationService = authorizationService;
        _logger = logger;
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
    [Authorize(Policy = nameof(PolicyNames.CreateCohort))]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> AddNewDraftApprenticeship(CreateCohortWithDraftApprenticeshipRequest request)
    {
        _logger.LogInformation("Adding apprentice: UseLearnerData {1} ", request.UseLearnerData);
        var redirectModel = await _modelMapper.Map<CreateCohortRedirectModel>(request);

        string action = redirectModel.RedirectTo switch
        {
            CreateCohortRedirectModel.RedirectTarget.SelectLearner => "SelectLearnerRecord",
            CreateCohortRedirectModel.RedirectTarget.SelectHowTo => nameof(SelectHowToAddApprentice),
            _ => nameof(SelectCourse)
        };

        request.CacheKey = redirectModel.CacheKey;
        return RedirectToAction(action, (action == "SelectLearnerRecord" ? "Learner" : "Cohort"), request.CloneBaseValues());
    }

    [HttpGet]
    [Route("add/select-how")]
    public IActionResult SelectHowToAddApprentice(CreateCohortWithDraftApprenticeshipRequest request)
    {
        var model = new SelectHowToAddFirstApprenticeshipJourneyViewModel
        {
            ProviderId = request.ProviderId,
            EmployerAccountLegalEntityPublicHashedId = request.EmployerAccountLegalEntityPublicHashedId,
            CacheKey = request.CacheKey
        };

        return View(model);
    }

    [HttpPost]
    [Route("add/select-how")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public ActionResult SelectHowToAddApprentice(SelectHowToAddFirstApprenticeshipJourneyViewModel model)
    {
        var redirectModel = new CreateCohortWithDraftApprenticeshipRequest
        {
            ProviderId = model.ProviderId,
            EmployerAccountLegalEntityPublicHashedId = model.EmployerAccountLegalEntityPublicHashedId,
            CacheKey = model.CacheKey,
            UseLearnerData = (model.Selection == AddFirstDraftApprenticeshipJourneyOptions.Ilr)
        };

        if (model.Selection == AddFirstDraftApprenticeshipJourneyOptions.Ilr)
        {
            return RedirectToAction("SelectLearnerRecord", "Learner", redirectModel);
        }
        return RedirectToAction("SelectCourse", redirectModel);
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
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> SelectCourse(SelectCourseRequest request)
    {
        var model = await _modelMapper.Map<SelectCourseViewModel>(request);
        return View(model);
    }

    [HttpPost]
    [Route("add/select-course")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> SelectCourse(SelectCourseViewModel model)
    {
        var request = await _modelMapper.Map<CreateCohortWithDraftApprenticeshipRequest>(model);
        return RedirectToAction(nameof(SelectDeliveryModel), request.CloneBaseValues());
    }

    [HttpGet]
    [Route("add/select-delivery-model")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> SelectDeliveryModel(CreateCohortWithDraftApprenticeshipRequest request)
    {
        var model = await _modelMapper.Map<SelectDeliveryModelViewModel>(request);

        if (model.DeliveryModels.Count > 1)
        {
            return View(model);
        }

        request.DeliveryModel = (DeliveryModel)model.DeliveryModels.FirstOrDefault();
        return RedirectToAction(nameof(AddDraftApprenticeship), request.CloneBaseValues());
    }

    [HttpPost]
    [Route("add/select-delivery-model")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> SetDeliveryModel(SelectDeliveryModelViewModel model)
    {
        var request = await _modelMapper.Map<CreateCohortWithDraftApprenticeshipRequest>(model);
        return RedirectToAction(nameof(AddDraftApprenticeship), request.CloneBaseValues());
    }

    [HttpGet]
    [Route("add/apprenticeship", Name  = RouteNames.CreateCohortAndAddFirstApprenticeship)]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    [ServiceFilter(typeof(UseCacheForValidationAttribute))]
    public async Task<IActionResult> AddDraftApprenticeship(CreateCohortWithDraftApprenticeshipRequest request)
    {
        var model = await _modelMapper.Map<AddDraftApprenticeshipViewModel>(request);
        return View(model);
    }

    [HttpPost]
    [Route("add/apprenticeship", Name = RouteNames.CohortAddApprenticeship)]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    [ServiceFilter(typeof(UseCacheForValidationAttribute))]
    public async Task<IActionResult> AddDraftApprenticeshipOrRoute(AddDraftApprenticeshipOrRoutePostRequest model)
    {
        var redirectModel = await _modelMapper.Map<AddDraftApprenticeshipRedirectModel>(model);

        if (redirectModel.RedirectTo == AddDraftApprenticeshipRedirectModel.RedirectTarget.SelectCourse)
        {
            return RedirectToAction(nameof(SelectCourse),
                new { redirectModel.ProviderId, redirectModel.CacheKey, IsEdit = true });
        }

        if (redirectModel.RedirectTo == AddDraftApprenticeshipRedirectModel.RedirectTarget.SelectDeliveryModel)
        {
            return RedirectToAction(nameof(SelectDeliveryModel),
                new { redirectModel.ProviderId, redirectModel.CacheKey, IsEdit = true });
        }

        if (redirectModel.RedirectTo == AddDraftApprenticeshipRedirectModel.RedirectTarget.OverlapWarning)
        {
            StoreDraftApprenticeshipState(model);
            var hashedApprenticeshipId = _encodingService.Encode(redirectModel.OverlappingApprenticeshipId.Value,
                EncodingType.ApprenticeshipId);
            return RedirectToAction("DraftApprenticeshipOverlapAlert", "OverlappingTrainingDateRequest", new
            {
                ProviderId = model.ProviderId,
                CacheKey = model.CacheKey,
                OverlapApprenticeshipHashedId = hashedApprenticeshipId,
                ReservationId = model.ReservationId,
                StartMonthYear = model.StartDate.MonthYear,
                CourseCode = model.CourseCode,
                DeliveryModel = model.DeliveryModel,
                EmployerAccountLegalEntityPublicHashedId = _encodingService.Encode(model.AccountLegalEntityId,
                    EncodingType.PublicAccountLegalEntityId)
            });
        }

        return await SaveDraftApprenticeship(model);
    }

    private async Task<IActionResult> SaveDraftApprenticeship(AddDraftApprenticeshipViewModel model)
    {
        var request = await _modelMapper.Map<CreateCohortRequest>(model);

        var response = await _mediator.Send(request);

        if (response.HasStandardOptions)
        {
            var draftApprenticeshipHashedId = _encodingService.Encode(response.DraftApprenticeshipId.Value,
                EncodingType.ApprenticeshipId);
            return RedirectToAction("SelectOptions", "DraftApprenticeship",
                new
                {
                    model.ProviderId,
                    DraftApprenticeshipHashedId = draftApprenticeshipHashedId,
                    response.CohortReference
                });
        }

        if (RecognisePriorLearningHelper.DoesDraftApprenticeshipRequireRpl(model))
        {
            var draftApprenticeshipHashedId = _encodingService.Encode(response.DraftApprenticeshipId.Value,
                EncodingType.ApprenticeshipId);
            return RedirectToRoute(RouteNames.RecognisePriorLearning,
                new { response.CohortReference, draftApprenticeshipHashedId, request.ProviderId });
        }

        return RedirectToAction(nameof(Details), new { model.ProviderId, response.CohortReference });
    }

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
    public async Task<IActionResult> ConfirmEmployer(ConfirmEmployerViewModel viewModel)
    {
        if (viewModel.Confirm.Value)
        {
            var model = await _modelMapper.Map<ConfirmEmployerRedirectModel>(viewModel);

            if (model.HasNoDeclaredStandards)
            {
                return RedirectToAction(nameof(NoDeclaredStandards), viewModel.ProviderId);
            }

            return Redirect(_urlHelper.ReservationsLink(
                $"{viewModel.ProviderId}/reservations/{viewModel.EmployerAccountLegalEntityPublicHashedId}/select?useLearnerData={viewModel.UseLearnerData}"));
        }
        return RedirectToAction(nameof(SelectEmployer), new { viewModel.ProviderId });
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
    public async Task<IActionResult> Delete([FromServices] IAuthenticationService authenticationService,
        DeleteCohortViewModel viewModel)
    {
        if (viewModel.Confirm.Value)
        {
            UserInfo userInfo = authenticationService.UserInfo;
            await _commitmentApiClient.DeleteCohort(viewModel.CohortId, userInfo);
            return RedirectToAction(nameof(Review), new { viewModel.ProviderId });
        }

        return RedirectToAction(nameof(Details), new { viewModel.ProviderId, viewModel.CohortReference });
    }

    [Route("{cohortReference}")]
    [Route("{cohortReference}/details")]
    [Authorize(Policy = nameof(PolicyNames.AccessCohort))]
    [Authorize(Policy = nameof(PolicyNames.HasViewerOrAbovePermission))]
    public async Task<IActionResult> Details(DetailsRequest request)
    {
        var viewModel = await _modelMapper.Map<DetailsViewModel>(request);

        if (viewModel.HasNoDeclaredStandards)
        {
            return RedirectToAction(nameof(NoDeclaredStandards), request.ProviderId);
        }

        return View(viewModel);
    }

    [HttpGet]
    [Route("NoDeclaredStandards")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public IActionResult NoDeclaredStandards()
    {
        return View();
    }

    [Route("{cohortReference}")]
    [Route("{cohortReference}/details")]
    [Authorize(Policy = nameof(PolicyNames.AccessCohort))]
    [Authorize(Policy = nameof(PolicyNames.HasViewerOrAbovePermission))]
    [HttpPost]
    public async Task<IActionResult> Details([FromServices] IPolicyAuthorizationWrapper authorizationService,
        DetailsViewModel viewModel)
    {
        switch (viewModel.Selection)
        {
            case CohortDetailsOptions.Send:
            case CohortDetailsOptions.Approve:
                {
                    await ValidateUserIsAuthorizedToAct(authorizationService);
                    var request = await _modelMapper.Map<AcknowledgementRequest>(viewModel);
                    return RedirectToAction(nameof(Acknowledgement), request);
                }
            case CohortDetailsOptions.ApprenticeRequest:
                {
                    return RedirectToAction(nameof(Review), new { viewModel.ProviderId });
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(viewModel.Selection));
        }
    }

    [HttpGet]
    [Route("{cohortReference}/Acknowledgement")]
    [Authorize(Policy = nameof(PolicyNames.AccessCohort))]
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
        return viewModel.Selection switch
        {
            AddDraftApprenticeshipEntryMethodOptions.ILR => RedirectToAction(nameof(BeforeYouContinue), new { viewModel.ProviderId }),
            AddDraftApprenticeshipEntryMethodOptions.BulkCsv => RedirectToAction(nameof(FileUploadInform), new { viewModel.ProviderId }),
            AddDraftApprenticeshipEntryMethodOptions.Manual => RedirectToAction(nameof(SelectAddDraftApprenticeshipJourney), new { viewModel.ProviderId }),
            _ => throw new InvalidOperationException()
        };
    }

    [HttpGet]
    [Route("add/before-you-continue")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public IActionResult BeforeYouContinue(BeforeYouContinueRequest request)
    {
        var model = new BeforeYouContinueViewModel { ProviderId = request.ProviderId };
        return View(model);
    }

    [HttpPost]
    [Route("add/before-you-continue")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public IActionResult BeforeYouContinue(BeforeYouContinueViewModel viewModel)
    {
        return RedirectToAction(nameof(SelectAddDraftApprenticeshipJourney), new { viewModel.ProviderId, UseLearnerData = true });
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
        var fileUploadLogId = await ValidateBulkUploadData(viewModel.ProviderId, viewModel.Attachment);
        viewModel.FileUploadLogId = fileUploadLogId;
        var request = await _modelMapper.Map<FileUploadReviewRequest>(viewModel);
        return RedirectToAction(nameof(FileUploadReview), request);
    }

    [HttpGet]
    [Route("add/file-upload/validate")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> FileUploadValidationErrors(FileUploadValidateErrorRequest request)
    {
        var viewModel = await _modelMapper.Map<FileUploadValidateViewModel>(request);
        if (viewModel.HasNoDeclaredStandards) return RedirectToAction(nameof(NoDeclaredStandards), request.ProviderId);
        return View(viewModel);
    }

    [HttpPost]
    [Route("add/file-upload/validate")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    [ServiceFilter(typeof(HandleBulkUploadValidationErrorsAttribute))]
    public async Task<IActionResult> FileUploadValidationErrors(FileUploadValidateViewModel viewModel)
    {
        var fileUploadLogId = await ValidateBulkUploadData(viewModel.ProviderId, viewModel.Attachment);
        viewModel.FileUploadLogId = fileUploadLogId;
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
        switch (viewModel.SelectedOption)
        {
            case FileUploadReviewOption.ApproveAndSend:
                var approveApiRequest = await _modelMapper.Map<BulkUploadAddAndApproveDraftApprenticeshipsRequest>(viewModel);
                var approvedResponse = await _outerApiService.BulkUploadAddAndApproveDraftApprenticeships(approveApiRequest);
                TempData.Put(Constants.BulkUpload.ApprovedApprenticeshipResponse, approvedResponse);
                return RedirectToAction(nameof(FileUploadSuccess), new { viewModel.ProviderId });

            case FileUploadReviewOption.SaveButDontSend:
                var apiRequest = await _modelMapper.Map<BulkUploadAddDraftApprenticeshipsRequest>(viewModel);
                var response = await _outerApiService.BulkUploadDraftApprenticeships(apiRequest);
                TempData.Put(Constants.BulkUpload.DraftApprenticeshipResponse, response);
                return RedirectToAction(nameof(FileUploadSuccessSaveDraft), new { viewModel.ProviderId });

            default:
                return RedirectToAction(nameof(FileUploadAmendedFile),
                    new FileUploadAmendedFileRequest
                    { ProviderId = viewModel.ProviderId, CacheRequestId = viewModel.CacheRequestId });
        }
    }

    [HttpGet]
    [Route("add/file-upload/success-save-draft", Name = RouteNames.SuccessSaveDraft)]
    public async Task<IActionResult> FileUploadSuccessSaveDraft(long providerId)
    {
        var response =
            TempData.GetButDontRemove<GetBulkUploadAddDraftApprenticeshipsResponse>(Constants.BulkUpload
                .DraftApprenticeshipResponse);
        var viewModel = await _modelMapper.Map<BulkUploadAddDraftApprenticeshipsViewModel>(response);
        viewModel.ProviderId = providerId;
        return View(viewModel);
    }

    [HttpGet]
    [Route("add/file-upload/success", Name = RouteNames.SuccessSendToEmployer)]
    public async Task<IActionResult> FileUploadSuccess(long providerId)
    {
        var response =
            TempData.GetButDontRemove<BulkUploadAddAndApproveDraftApprenticeshipsResponse>(Constants.BulkUpload
                .ApprovedApprenticeshipResponse);
        var viewModel = await _modelMapper.Map<BulkUploadAddAndApproveDraftApprenticeshipsViewModel>(response);
        viewModel.ProviderId = providerId;
        return View(viewModel);
    }

    [HttpGet]
    [Route("add/file-upload/discard-file")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public IActionResult FileUploadDiscard(FileDiscardRequest fileDiscardRequest)
    {
        var viewModel = new FileDiscardViewModel
        { CacheRequestId = fileDiscardRequest.CacheRequestId, ProviderId = fileDiscardRequest.ProviderId };
        return View(viewModel);
    }

    [HttpPost]
    [Route("add/file-upload/discard-file")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public IActionResult FileUploadDiscard(FileDiscardViewModel viewModel)
    {
        if (viewModel.FileDiscardConfirmed != null && (bool)viewModel.FileDiscardConfirmed)
        {
            return RedirectToAction(nameof(FileUploadReviewDelete),
                new FileUploadReviewDeleteRequest
                {
                    ProviderId = viewModel.ProviderId,
                    CacheRequestId = viewModel.CacheRequestId,
                    RedirectTo = FileUploadReviewDeleteRedirect.SuccessDiscardFile
                });
        }

        return RedirectToAction(nameof(FileUploadReview),
            new { ProviderId = viewModel.ProviderId, CacheRequestId = viewModel.CacheRequestId });
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
            return RedirectToAction(nameof(FileUploadStart),
                new SelectAddDraftApprenticeshipJourneyRequest { ProviderId = viewModel.ProviderId });
        }

        return RedirectToAction(nameof(FileUploadReview),
            new FileUploadReviewRequest
            { CacheRequestId = viewModel.CacheRequestId, ProviderId = viewModel.ProviderId });
    }

    [HttpGet]
    [Route("add/select-journey")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> SelectAddDraftApprenticeshipJourney(
        SelectAddDraftApprenticeshipJourneyRequest request)
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
            return RedirectToAction(nameof(ChooseCohort), new { ProviderId = viewModel.ProviderId, viewModel.UseLearnerData });
        }

        if (viewModel.Selection == AddDraftApprenticeshipJourneyOptions.NewCohort)
        {
            return RedirectToAction(nameof(SelectEmployer), new { ProviderId = viewModel.ProviderId, viewModel.UseLearnerData });
        }
        
        throw new InvalidOperationException();
    }

    [HttpGet]
    [Route("add/file-upload/review-cohort")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> FileUploadReviewApprentices(
        FileUploadReviewApprenticeRequest reviewApprenticeRequest)
    {
        var viewModel = await _modelMapper.Map<FileUploadReviewApprenticeViewModel>(reviewApprenticeRequest);
        return View(viewModel);
    }

    private async Task ValidateUserIsAuthorizedToAct(IPolicyAuthorizationWrapper authorizationService)
    {
        var result =
            await authorizationService.IsAuthorized(User, PolicyNames.HasContributorWithApprovalOrAbovePermission);

        if (!result)
        {
            throw new UnauthorizedActionException();
        }
    }

    private void StoreDraftApprenticeshipState(AddDraftApprenticeshipViewModel model)
    {
        TempData.Put(nameof(AddDraftApprenticeshipViewModel), model);
    }

    private async Task<long> ValidateBulkUploadData(long providerId, IFormFile attachment)
    {
        var bulkValidate = new FileUploadValidateDataRequest { Attachment = attachment, ProviderId = providerId };
        var response = await _mediator.Send(bulkValidate);
        return response.LogId;
    }
}