using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Attributes;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Cookies;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using SFA.DAS.ProviderUrlHelper;
using SelectDeliveryModelViewModel = SFA.DAS.ProviderCommitments.Web.Models.Apprentice.SelectDeliveryModelViewModel;

namespace SFA.DAS.ProviderCommitments.Web.Controllers;

[Route("{providerId}/apprentices")]
[SetNavigationSection(NavigationSection.ManageApprentices)]
public class ApprenticeController(
    IModelMapper modelMapper,
    Interfaces.ICookieStorageService<IndexRequest> cookieStorage,
    ICommitmentsApiClient commitmentsApiClient,
    IOuterApiService outerApiService,
    ICacheStorageService cacheStorage)
    : Controller
{
    public const string ChangesApprovedFlashMessage = "Changes approved";
    public const string ChangesRejectedFlashMessage = "Changes rejected";
    public const string ChangesUndoneFlashMessage = "Changes undone";
    private const string ApprenticeChangesSentToEmployer = "Change saved and sent to employer to approve";
    private const string ApprenticeUpdated = "Change saved (re-approval not required)";
    private const string ViewModelForEdit = "ViewModelForEdit";

    [Route("", Name = RouteNames.ApprenticesIndex)]
    public async Task<IActionResult> Index(IndexRequest request)
    {
        IndexRequest savedRequest = null;

        if (request.FromSearch)
        {
            savedRequest = cookieStorage.Get(CookieNames.ManageApprentices);

            if (savedRequest != null)
            {
                request = savedRequest;
            }
        }

        if (savedRequest == null)
        {
            cookieStorage.Update(CookieNames.ManageApprentices, request);
        }

        var viewModel = await modelMapper.Map<IndexViewModel>(request);
        viewModel.SortedByHeader();

        return View(viewModel);
    }

    [Route("{apprenticeshipHashedId}", Name = RouteNames.ApprenticeDetail)]
    public async Task<IActionResult> Details(DetailsRequest request, ApprenticeDetailsBanners banners = 0)
    {
        var viewModel = await modelMapper.Map<DetailsViewModel>(request);
        viewModel.ShowBannersFlags = banners;
        return View(viewModel);
    }

    [HttpGet]
    [Route("{apprenticeshipHashedId}/changes/view", Name = RouteNames.ApprenticeViewApprenticeshipUpdates)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    public async Task<IActionResult> ViewApprenticeshipUpdates(ViewApprenticeshipUpdatesRequest request)
    {
        var viewModel = await modelMapper.Map<ViewApprenticeshipUpdatesViewModel>(request);

        return View(viewModel);
    }

    [HttpPost]
    [Route("{apprenticeshipHashedId}/changes/view")]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> ViewApprenticeshipUpdates([FromServices] IAuthenticationService authenticationService, ViewApprenticeshipUpdatesViewModel viewModel)
    {
        if (viewModel.UndoChanges.Value)
        {
            var request = new UndoApprenticeshipUpdatesRequest
            {
                ApprenticeshipId = viewModel.ApprenticeshipId,
                ProviderId = viewModel.ProviderId,
                UserInfo = authenticationService.UserInfo
            };

            await commitmentsApiClient.UndoApprenticeshipUpdates(viewModel.ApprenticeshipId, request);

            TempData.AddFlashMessage(ChangesUndoneFlashMessage, ITempDataDictionaryExtensions.FlashMessageLevel.Success);
        }

        return RedirectToRoute(RouteNames.ApprenticeDetail, new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId });
    }

    [HttpGet]
    [Route("{apprenticeshipHashedId}/changes/review", Name = RouteNames.ApprenticeReviewApprenticeshipUpdates)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    public async Task<IActionResult> ReviewApprenticeshipUpdates(ReviewApprenticeshipUpdatesRequest request)
    {
        var viewModel = await modelMapper.Map<ReviewApprenticeshipUpdatesViewModel>(request);

        if (!viewModel.IsValidCourseCode)
        {
            ModelState.AddModelError("IsValidCourseCode", "This training course has not been declared");
        }

        return View(viewModel);
    }

    [HttpPost]
    [Route("{apprenticeshipHashedId}/changes/review")]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> ReviewApprenticeshipUpdates([FromServices] IAuthenticationService authenticationService, ReviewApprenticeshipUpdatesViewModel viewModel, [FromServices] ILinkGenerator urlHelper)
    {
        if (!viewModel.IsValidCourseCode)
        {
            if (viewModel.ApproveAddStandardToTraining.Value)
            {
                var reservationUrl = $"{viewModel.ProviderId}/review-your-details";
                return Redirect(urlHelper.CourseManagementLink(reservationUrl));
            }
            if (!viewModel.ApproveAddStandardToTraining.Value)
            {
                var request = new RejectApprenticeshipUpdatesRequest
                {
                    ApprenticeshipId = viewModel.ApprenticeshipId,
                    ProviderId = viewModel.ProviderId,
                    UserInfo = authenticationService.UserInfo
                };

                await commitmentsApiClient.RejectApprenticeshipUpdates(viewModel.ApprenticeshipId, request);

                TempData.AddFlashMessage(ChangesRejectedFlashMessage, ITempDataDictionaryExtensions.FlashMessageLevel.Success);
            }
        }
        else
        {
            if (viewModel.ApproveChanges.Value)
            {
                var request = new AcceptApprenticeshipUpdatesRequest
                {
                    ApprenticeshipId = viewModel.ApprenticeshipId,
                    ProviderId = viewModel.ProviderId,
                    UserInfo = authenticationService.UserInfo
                };

                await commitmentsApiClient.AcceptApprenticeshipUpdates(viewModel.ApprenticeshipId, request);

                TempData.AddFlashMessage(ChangesApprovedFlashMessage, ITempDataDictionaryExtensions.FlashMessageLevel.Success);
            }
            else if (!viewModel.ApproveChanges.Value)
            {
                var request = new RejectApprenticeshipUpdatesRequest
                {
                    ApprenticeshipId = viewModel.ApprenticeshipId,
                    ProviderId = viewModel.ProviderId,
                    UserInfo = authenticationService.UserInfo
                };

                await commitmentsApiClient.RejectApprenticeshipUpdates(viewModel.ApprenticeshipId, request);

                TempData.AddFlashMessage(ChangesRejectedFlashMessage, ITempDataDictionaryExtensions.FlashMessageLevel.Success);
            }
        }
        return RedirectToRoute(RouteNames.ApprenticeDetail, new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId });
    }

    [HttpGet]
    [Route("download", Name = RouteNames.DownloadApprentices)]
    public async Task<IActionResult> Download(DownloadRequest request)
    {
        var downloadViewModel = await modelMapper.Map<DownloadViewModel>(request);
        return File(downloadViewModel.Content, downloadViewModel.ContentType, downloadViewModel.Name);
    }

    [HttpGet]
    [Route("{apprenticeshipHashedId}/change-employer/confirm-employer", Name = RouteNames.ApprenticeConfirmEmployer)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> ConfirmEmployer(ConfirmEmployerRequest request)
    {
        var viewModel = await modelMapper.Map<ConfirmEmployerViewModel>(request);
        return View(viewModel);
    }

    [HttpPost]
    [Route("{apprenticeshipHashedId}/change-employer/confirm-employer", Name = RouteNames.ApprenticeConfirmEmployer)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> ConfirmEmployer(ConfirmEmployerViewModel viewModel)
    {
        if (viewModel.Confirm.Value)
        {
            var request = await modelMapper.Map<SelectDeliveryModelRequest>(viewModel);
            return RedirectToAction(nameof(SelectDeliveryModel), request);
        }

        return RedirectToAction(nameof(SelectEmployer), new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId });
    }

    [HttpGet]
    [Route("{apprenticeshipHashedId}/change-employer/select-delivery-model", Name = RouteNames.ApprenticeSelectDeliveryModel)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> SelectDeliveryModel(SelectDeliveryModelRequest request)
    {
        var viewModel = await modelMapper.Map<SelectDeliveryModelViewModel>(request);
        if (viewModel.DeliveryModels.Count > 1)
        {
            return View(viewModel);
        }

        return RedirectToAction(nameof(TrainingDates), new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId, viewModel.CacheKey });
    }

    [HttpPost]
    [Route("{apprenticeshipHashedId}/change-employer/select-delivery-model", Name = RouteNames.ApprenticeSelectDeliveryModel)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> SelectDeliveryModel(SelectDeliveryModelViewModel viewModel)
    {
        if (viewModel.IsEdit)
        {
            var confirmRequest = await modelMapper.Map<ConfirmRequest>(viewModel);
            return RedirectToAction(nameof(Confirm), confirmRequest);
        }

        var request = await modelMapper.Map<TrainingDatesRequest>(viewModel);
        return RedirectToAction(nameof(TrainingDates), request);
    }

    [HttpGet]
    [Route("{apprenticeshipHashedId}/change-employer/training-dates", Name = RouteNames.ApprenticeTrainingDates)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> TrainingDates(TrainingDatesRequest request)
    {
        var viewModel = await modelMapper.Map<TrainingDatesViewModel>(request);
        return View(viewModel);
    }

    [HttpPost]
    [Route("{apprenticeshipHashedId}/change-employer/training-dates", Name = RouteNames.ApprenticeTrainingDates)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> TrainingDates(TrainingDatesViewModel viewModel)
    {
        await ValidateChangeOfEmployerOverlap(viewModel);

        var request = await modelMapper.Map<PriceRequest>(viewModel);
        return RedirectToAction(nameof(Price), "Apprentice", request);
    }

    [HttpGet]
    [Route("{apprenticeshipHashedId}/change-employer", Name = RouteNames.ChangeEmployer)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    public async Task<IActionResult> ChangeEmployer(ChangeEmployerRequest request)
    {
        var viewModel = await modelMapper.Map<IChangeEmployerViewModel>(request);

        if (viewModel is InformViewModel)
        {
            return RedirectToRoute(RouteNames.ChangeEmployerInform, new { request.ProviderId, request.ApprenticeshipHashedId });
        }

        TempData["ChangeEmployerModel"] = JsonConvert.SerializeObject(viewModel);
            
        return RedirectToRoute(RouteNames.ChangeEmployerDetails, new {request.ProviderId, request.ApprenticeshipHashedId});
    }

    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    [Route("{apprenticeshipHashedId}/change-employer/inform", Name = RouteNames.ChangeEmployerInform)]
    public async Task<IActionResult> ChangeEmployerInform(ChangeEmployerInformRequest request)
    {
        var viewModel = await modelMapper.Map<InformViewModel>(request);
        return View("Inform", viewModel);
    }

    [Route("{apprenticeshipHashedId}/change-employer/change-employer-details", Name = RouteNames.ChangeEmployerDetails)]
    public IActionResult ChangeEmployerDetails()
    {
        return View("ChangeEmployerRequestDetails", JsonConvert.DeserializeObject<ChangeEmployerRequestDetailsViewModel>(TempData["ChangeEmployerModel"].ToString()));
    }

    [HttpGet]
    [Route("{apprenticeshipHashedId}/change-employer/select-employer", Name = RouteNames.ApprenticeSelectEmployer)]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    public async Task<IActionResult> SelectEmployer(SelectEmployerRequest request)
    {
        var viewModel = await modelMapper.Map<SelectEmployerViewModel>(request);
        return View(viewModel);
    }

    [HttpGet]
    [Route("{apprenticeshipHashedId}/change-employer/price", Name = RouteNames.ApprenticePrice)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> Price(PriceRequest request)
    {
        var model = await modelMapper.Map<PriceViewModel>(request);
        return View(model);
    }

    [HttpPost]
    [Route("{apprenticeshipHashedId}/change-employer/price", Name = RouteNames.ApprenticePrice)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> Price(PriceViewModel viewModel)
    {
        if (viewModel.ApprenticeshipStatus == ApprenticeshipStatus.Stopped
            && await ValidateApprenticeshipDatesForChangeOfEmployer(viewModel.CacheKey, viewModel.ApprenticeshipId))
        {
            var request = await modelMapper.Map<ConfirmRequest>(viewModel);
            return RedirectToRoute(RouteNames.ApprenticeConfirm, request);
        }

        var overlapRequest = await modelMapper.Map<ChangeOfEmployerOverlapAlertRequest>(viewModel);
        return RedirectToRoute(RouteNames.ChangeEmployerOverlapAlert, overlapRequest);
    }

    [HttpGet]
    [Route("{apprenticeshipHashedId}/change-employer/confirm", Name = RouteNames.ApprenticeConfirm)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> Confirm(ConfirmRequest request)
    {
        var viewModel = await modelMapper.Map<ConfirmViewModel>(request);
        return View(viewModel);
    }

    [HttpPost]
    [Route("{apprenticeshipHashedId}/change-employer/confirm", Name = RouteNames.ApprenticeConfirm)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> Confirm(ConfirmViewModel viewModel)
    {
        var request = await modelMapper.Map<SentRequest>(viewModel);
        TempData[nameof(ConfirmViewModel.NewEmployerName)] = viewModel.NewEmployerName;
            
        return RedirectToRoute(RouteNames.ApprenticeSent, request);
    }

    [HttpGet]
    [Route("{apprenticeshipHashedId}/change-employer/overlap-alert", Name = RouteNames.ChangeEmployerOverlapAlert)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> ChangeOfEmployerOverlapAlert(ChangeOfEmployerOverlapAlertRequest request)
    {
        var model = await modelMapper.Map<ChangeOfEmployerOverlapAlertViewModel>(request);
        return View(model);
    }

    [HttpPost]
    [Route("{apprenticeshipHashedId}/change-employer/overlap-alert", Name = RouteNames.ChangeEmployerOverlapAlert)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> ChangeOfEmployerOverlapAlert(ChangeOfEmployerOverlapAlertViewModel viewModel)
    {
        var model = await modelMapper.Map<OverlapOptionsForChangeEmployerRequest>(viewModel);

        return RedirectToAction(ControllerConstants.OverlappingTrainingDateRequestController.Actions.OverlapOptionsForChangeEmployer, ControllerConstants.OverlappingTrainingDateRequestController.Name, model);
    }

    [HttpGet]
    [Route("{apprenticeshipHashedId}/change-employer/sent", Name = RouteNames.ApprenticeSent)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public IActionResult Sent()
    {
        var model = TempData[nameof(ConfirmViewModel.NewEmployerName)] as string;
        return View(nameof(Sent), model);
    }

    [HttpGet]
    [Route("{apprenticeshipHashedId}/edit", Name = RouteNames.EditApprenticeship)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> EditApprenticeship(EditApprenticeshipRequest request)
    {
        var viewModel = TempData.Get<EditApprenticeshipRequestViewModel>(ViewModelForEdit) ?? await modelMapper.Map<EditApprenticeshipRequestViewModel>(request);
        return View(viewModel);
    }

    [HttpPost]
    [Route("{apprenticeshipHashedId}/edit")]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> EditApprenticeship(string changeCourse, string changeDeliveryModel, EditApprenticeshipRequestViewModel viewModel)
    {
        if (changeCourse == "Edit" || changeDeliveryModel == "Edit")
        {
            TempData.Put(ViewModelForEdit, viewModel);
            return RedirectToAction(changeCourse == "Edit" ? nameof(EditApprenticeshipCourse) : nameof(SelectDeliveryModelForEdit), new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId });
        }

        var editRequest = await modelMapper.Map<ValidateEditApprenticeshipRequest>(
            viewModel);
            
        var response = await outerApiService.EditApprenticeship(
            viewModel.ProviderId,
            viewModel.ApprenticeshipId,
            editRequest);


        viewModel.HasOptions = response.HasOptions;
        viewModel.Version = response.Version;

        viewModel.Option = response.CourseOrStartDateChange ? null : viewModel.Option;

        
        TempData.Put("EditApprenticeshipRequestViewModel", viewModel);
        if (response.HasOptions)
        {
            return RedirectToAction(nameof(ChangeOption), new { apprenticeshipHashedId = viewModel.ApprenticeshipHashedId, providerId = viewModel.ProviderId });
        }
            
        return RedirectToAction(nameof(ConfirmEditApprenticeship), new { apprenticeshipHashedId = viewModel.ApprenticeshipHashedId, providerId = viewModel.ProviderId });
    }

    [HttpGet]
    [Route("{apprenticeshipHashedId}/edit/select-course")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> EditApprenticeshipCourse(EditApprenticeshipRequest request)
    {
        var model = await modelMapper.Map<EditApprenticeshipCourseViewModel>(request);
        return View(model);
    }

    [HttpPost]
    [Route("{apprenticeshipHashedId}/edit/select-course")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> SetCourseForEdit(EditApprenticeshipCourseViewModel model)
    {
        var request = await modelMapper.Map<BaseApprenticeshipRequest>(model);

        return RedirectToAction(nameof(SelectDeliveryModelForEdit), new { request.ProviderId, request.ApprenticeshipHashedId });
    }

    [HttpGet]
    [Route("{apprenticeshipHashedId}/edit/select-delivery-model")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public async Task<IActionResult> SelectDeliveryModelForEdit(EditApprenticeshipRequest request)
    {
        var draft = TempData.GetButDontRemove<EditApprenticeshipRequestViewModel>(ViewModelForEdit);
        var model = await modelMapper.Map<EditApprenticeshipDeliveryModelViewModel>(draft);

        if (model.DeliveryModels.Count > 1)
        {
            return View(model);
        }
        draft.DeliveryModel = (DeliveryModel)model.DeliveryModels.FirstOrDefault();
        TempData.Put(ViewModelForEdit, draft);

        return RedirectToAction(nameof(EditApprenticeship), new { request.ProviderId, request.ApprenticeshipHashedId });
    }

    [HttpPost]
    [Route("{DraftApprenticeshipHashedId}/edit/select-delivery-model")]
    [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
    public IActionResult SetDeliveryModelForEdit(EditApprenticeshipDeliveryModelViewModel model)
    {
        if (model.DeliveryModel == null)
        {
            throw new CommitmentsApiModelException(new List<ErrorDetail>
                {new ErrorDetail("DeliveryModel", "You must select the apprenticeship delivery model")});
        }

        var draft = TempData.GetButDontRemove<EditApprenticeshipRequestViewModel>(ViewModelForEdit);
        draft.DeliveryModel = (DeliveryModel)model.DeliveryModel.Value;
        TempData.Put(ViewModelForEdit, draft);
        return RedirectToAction(nameof(EditApprenticeship), new { draft.ProviderId, draft.ApprenticeshipHashedId });
    }

    [HttpGet]
    [Route("{apprenticeshipHashedId}/edit/change-version", Name = RouteNames.ChangeVersion)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> ChangeVersion(ChangeVersionRequest request)
    {
        var viewModel = await modelMapper.Map<ChangeVersionViewModel>(request);

        var editViewModel = TempData.GetButDontRemove<EditApprenticeshipRequestViewModel>("EditApprenticeshipRequestViewModel");

        if (editViewModel != null)
        {
            viewModel.SelectedVersion = editViewModel.Version;
        }

        return View(viewModel);
    }

    [HttpPost]
    [Route("{apprenticeshipHashedId}/edit/change-version", Name = RouteNames.ChangeVersion)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> ChangeVersion(ChangeVersionViewModel viewModel)
    {
        var editApprenticeshipRequestViewModel = await modelMapper.Map<EditApprenticeshipRequestViewModel>(viewModel);

        TempData.Put("EditApprenticeshipRequestViewModel", editApprenticeshipRequestViewModel);

        if (editApprenticeshipRequestViewModel.HasOptions)
        {
            return RedirectToAction(nameof(ChangeOption), new { apprenticeshipHashedId = viewModel.ApprenticeshipHashedId, providerId = viewModel.ProviderId });
        }

        return RedirectToAction(nameof(ConfirmEditApprenticeship), new { apprenticeshipHashedId = viewModel.ApprenticeshipHashedId, providerId = viewModel.ProviderId });
    }

    [HttpGet]
    [Route("{apprenticeshipHashedId}/edit/change-option", Name = RouteNames.ChangeOption)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> ChangeOption(ChangeOptionRequest request)
    {
        var viewModel = await modelMapper.Map<ChangeOptionViewModel>(request);

        return View(viewModel);
    }

    [HttpPost]
    [Route("{apprenticeshipHashedId}/edit/change-option")]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> ChangeOption(ChangeOptionViewModel viewModel)
    {
        var editViewModel = await modelMapper.Map<EditApprenticeshipRequestViewModel>(viewModel);

        TempData.Put("EditApprenticeshipRequestViewModel", editViewModel);

        return RedirectToAction(nameof(ConfirmEditApprenticeship), new { apprenticeshipHashedId = viewModel.ApprenticeshipHashedId, providerId = viewModel.ProviderId });
    }

    [HttpGet]
    [Route("{apprenticeshipHashedId}/edit/confirm")]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> ConfirmEditApprenticeship()
    {
        var editApprenticeshipRequestViewModel = TempData.GetButDontRemove<EditApprenticeshipRequestViewModel>("EditApprenticeshipRequestViewModel");
        var viewModel = await modelMapper.Map<ConfirmEditApprenticeshipViewModel>(editApprenticeshipRequestViewModel);

        return View(viewModel);
    }

    [HttpPost]
    [Route("{apprenticeshipHashedId}/edit/confirm")]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> ConfirmEditApprenticeship(ConfirmEditApprenticeshipViewModel viewModel)
    {
        if (viewModel.ConfirmChanges.Value)
        {
            var request = await modelMapper.Map<ConfirmEditApprenticeshipRequest>(viewModel);
            var result = await outerApiService.ConfirmEditApprenticeship(viewModel.ProviderId, viewModel.ApprenticeshipId, request);

            if (result.NeedReapproval)
            {
                TempData.AddFlashMessage(ApprenticeChangesSentToEmployer, ITempDataDictionaryExtensions.FlashMessageLevel.Info);
            }
            else
            {
                TempData.AddFlashMessage(ApprenticeUpdated, ITempDataDictionaryExtensions.FlashMessageLevel.Info);
            }
        }

        TempData.Remove("EditApprenticeshipRequestViewModel");

        return RedirectToAction(nameof(Details), new { apprenticeshipHashedId = viewModel.ApprenticeshipHashedId, providerId = viewModel.ProviderId });
    }

    [HttpGet]
    [Route("{apprenticeshipHashedId}/cancel-change-of-circumstance", Name = RouteNames.CancelInProgressChangeOfCircumstance)]
    public IActionResult CancelChangeOfCircumstance(CancelChangeOfCircumstanceRequest request)
    {
        TempData.Remove("EditApprenticeshipRequestViewModel");

        return RedirectToAction(nameof(Details), new { request.ProviderId, request.ApprenticeshipHashedId });
    }

    [HttpGet]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Route("{apprenticeshipHashedId}/datalock/requestrestart", Name = RouteNames.RequestRestart)]
    public async Task<IActionResult> DataLockRequestRestart(DataLockRequestRestartRequest request)
    {
        var viewModel = await modelMapper.Map<DataLockRequestRestartViewModel>(request);

        return View(viewModel);
    }

    [HttpPost]
    [Route("{apprenticeshipHashedId}/datalock/requestrestart", Name = RouteNames.RequestRestart)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public IActionResult DataLockRequestRestart(DataLockRequestRestartViewModel viewModel)
    {
        if (viewModel.SubmitStatusViewModel.HasValue && viewModel.SubmitStatusViewModel.Value == SubmitStatusViewModel.Confirm)
        {
            return RedirectToAction(nameof(ConfirmRestart), new DatalockConfirmRestartRequest { ApprenticeshipHashedId = viewModel.ApprenticeshipHashedId, ProviderId = viewModel.ProviderId });
        }

        return RedirectToAction(nameof(Details), new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId });
    }

    [HttpGet]
    [Route("{apprenticeshipHashedId}/datalock/confirmrestart", Name = RouteNames.ConfirmRestart)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    public IActionResult ConfirmRestart(DatalockConfirmRestartRequest request)
    {
        var viewModel = new DatalockConfirmRestartViewModel { ApprenticeshipHashedId = request.ApprenticeshipHashedId, ProviderId = request.ProviderId };
        return View("DataLockConfirmRestart", viewModel);
    }

    [HttpPost]
    [Route("{apprenticeshipHashedId}/datalock/confirmrestart", Name = RouteNames.ConfirmRestart)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<ActionResult> ConfirmRestart(DatalockConfirmRestartViewModel viewModel)
    {
        if (viewModel.SendRequestToEmployer.HasValue && viewModel.SendRequestToEmployer.Value)
        {
            var request = await modelMapper.Map<TriageDataLocksRequest>(viewModel);
            await commitmentsApiClient.TriageDataLocks(viewModel.ApprenticeshipId, request);
        }

        return RedirectToAction(nameof(Details), new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId });
    }

    [HttpGet]
    [Route("{apprenticeshipHashedId}/datalock", Name = RouteNames.UpdateDateLock)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    public async Task<IActionResult> UpdateDataLock(UpdateDateLockRequest request)
    {
        var viewModel = await modelMapper.Map<UpdateDateLockViewModel>(request);
        return View("UpdateDataLock", viewModel);
    }

    [HttpPost]
    [Route("{apprenticeshipHashedId}/datalock", Name = RouteNames.UpdateDateLock)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public IActionResult UpdateDataLock(UpdateDateLockViewModel viewModel)
    {
        if (viewModel.SubmitStatusViewModel == SubmitStatusViewModel.Confirm)
        {
            return RedirectToAction(nameof(ConfirmDataLockChanges), new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId });
        }

        return RedirectToAction(nameof(Details), new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId });
    }

    [HttpGet]
    [Route("{apprenticeshipHashedId}/datalock/confirm", Name = RouteNames.UpdateDataLockConfirm)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    public async Task<ActionResult> ConfirmDataLockChanges(ConfirmDataLockChangesRequest request)
    {
        var viewModel = await modelMapper.Map<ConfirmDataLockChangesViewModel>(request);
        return View(viewModel);
    }

    [HttpPost]
    [Route("{apprenticeshipHashedId}/datalock/confirm", Name = RouteNames.UpdateDataLockConfirm)]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
    public async Task<IActionResult> ConfirmDataLockChanges(ConfirmDataLockChangesViewModel viewModel)
    {
        if (viewModel.SubmitStatusViewModel != null && viewModel.SubmitStatusViewModel.Value == SubmitStatusViewModel.Confirm)
        {
            var request = await modelMapper.Map<TriageDataLocksRequest>(viewModel);
            await commitmentsApiClient.TriageDataLocks(viewModel.ApprenticeshipId, request);
        }

        return RedirectToAction(nameof(Details), new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId });
    }

    [HttpGet]
    [Route("{apprenticeshipHashedId}/details/resend-email-invitation")]
    [Authorize(Policy = nameof(PolicyNames.AccessApprenticeship))]
    public async Task<IActionResult> ResendEmailInvitation([FromServices] IAuthenticationService authenticationService, ResendEmailInvitationRequest request)
    {
        try
        {
            await commitmentsApiClient.ResendApprenticeshipInvitation(request.ApprenticeshipId, new SaveDataRequest { UserInfo = authenticationService.UserInfo });

            TempData.AddFlashMessage("The invitation email has been resent.", null, ITempDataDictionaryExtensions.FlashMessageLevel.Success);
        }
        catch { }

        return RedirectToAction(nameof(Details), new
        {
            ProviderId = request.ProviderId,
            ApprenticeshipHashedId = request.ApprenticeshipHashedId
        });
    }

    private async Task ValidateChangeOfEmployerOverlap(TrainingDatesViewModel model)
    {
        var apimRequest = await modelMapper.Map<ValidateChangeOfEmployerOverlapApimRequest>(model);
        await outerApiService.ValidateChangeOfEmployerOverlap(apimRequest);
    }

    private async Task<bool> ValidateApprenticeshipDatesForChangeOfEmployer(Guid cacheKey, long apprenticeshipId)
    {
        var apprenticeship = await commitmentsApiClient.GetApprenticeship(apprenticeshipId);
        var cacheItem = await cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(cacheKey);
        var startDate = new MonthYearModel(cacheItem.StartDate).Date.Value;

        return startDate >= apprenticeship.StopDate.Value;
    }
}