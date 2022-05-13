using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFA.DAS.Authorization.CommitmentPermissions.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Attributes;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Cookies;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}/apprentices")]
    [SetNavigationSection(NavigationSection.ManageApprentices)]
    public class ApprenticeController : Controller
    {
        private readonly ICookieStorageService<IndexRequest> _cookieStorage;
        private readonly IModelMapper _modelMapper;
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public const string ChangesApprovedFlashMessage = "Changes approved";
        public const string ChangesRejectedFlashMessage = "Changes rejected";
        public const string ChangesUndoneFlashMessage = "Changes undone";
        private const string ApprenticeChangesSentToEmployer = "Change saved and sent to employer to approve";
        private const string ApprenticeUpdated = "Change saved (re-approval not required)";
        private const string ViewModelForEdit = "ViewModelForEdit";

        public ApprenticeController(IModelMapper modelMapper, ICookieStorageService<IndexRequest> cookieStorage, ICommitmentsApiClient commitmentsApiClient)
        {
            _modelMapper = modelMapper;
            _cookieStorage = cookieStorage;
            _commitmentsApiClient = commitmentsApiClient;
        }

        [Route("", Name = RouteNames.ApprenticesIndex)]
        public async Task<IActionResult> Index(IndexRequest request)
        {
            IndexRequest savedRequest = null;

            if (request.FromSearch)
            {
                savedRequest = _cookieStorage.Get(CookieNames.ManageApprentices);

                if (savedRequest != null)
                {
                    request = savedRequest;
                }
            }

            if (savedRequest == null)
            {
                _cookieStorage.Update(CookieNames.ManageApprentices, request);
            }

            var viewModel = await _modelMapper.Map<IndexViewModel>(request);
            viewModel.SortedByHeader();

            return View(viewModel);
        }

        [Route("{apprenticeshipHashedId}", Name = RouteNames.ApprenticeDetail)]
        public async Task<IActionResult> Details(DetailsRequest request)
        {
            var viewModel = await _modelMapper.Map<DetailsViewModel>(request);
            return View(viewModel);
        }

        [HttpGet]
        [Route("{apprenticeshipHashedId}/changes/view", Name = RouteNames.ApprenticeViewApprenticeshipUpdates)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        public async Task<IActionResult> ViewApprenticeshipUpdates(ViewApprenticeshipUpdatesRequest request)
        {
            var viewModel = await _modelMapper.Map<ViewApprenticeshipUpdatesViewModel>(request);

            return View(viewModel);
        }

        [HttpPost]
        [Route("{apprenticeshipHashedId}/changes/view")]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
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

                await _commitmentsApiClient.UndoApprenticeshipUpdates(viewModel.ApprenticeshipId, request);

                TempData.AddFlashMessage(ChangesUndoneFlashMessage, ITempDataDictionaryExtensions.FlashMessageLevel.Success);
            }

            return RedirectToRoute(RouteNames.ApprenticeDetail, new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId });
        }

        [HttpGet]
        [Route("{apprenticeshipHashedId}/changes/review", Name = RouteNames.ApprenticeReviewApprenticeshipUpdates)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        public async Task<IActionResult> ReviewApprenticeshipUpdates(ReviewApprenticeshipUpdatesRequest request)
        {
            var viewModel = await _modelMapper.Map<ReviewApprenticeshipUpdatesViewModel>(request);

            return View(viewModel);
        }

        [HttpPost]
        [Route("{apprenticeshipHashedId}/changes/review")]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        public async Task<IActionResult> ReviewApprenticeshipUpdates([FromServices] IAuthenticationService authenticationService, ReviewApprenticeshipUpdatesViewModel viewModel)
        {
            if (viewModel.ApproveChanges.Value)
            {
                var request = new AcceptApprenticeshipUpdatesRequest
                {
                    ApprenticeshipId = viewModel.ApprenticeshipId,
                    ProviderId = viewModel.ProviderId,
                    UserInfo = authenticationService.UserInfo
                };

                await _commitmentsApiClient.AcceptApprenticeshipUpdates(viewModel.ApprenticeshipId, request);

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

                await _commitmentsApiClient.RejectApprenticeshipUpdates(viewModel.ApprenticeshipId, request);

                TempData.AddFlashMessage(ChangesRejectedFlashMessage, ITempDataDictionaryExtensions.FlashMessageLevel.Success);
            }

            return RedirectToRoute(RouteNames.ApprenticeDetail, new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId });
        }

        [HttpGet]
        [Route("download", Name = RouteNames.DownloadApprentices)]
        public async Task<IActionResult> Download(DownloadRequest request)
        {
            var downloadViewModel = await _modelMapper.Map<DownloadViewModel>(request);
            return File(downloadViewModel.Content, downloadViewModel.ContentType, downloadViewModel.Name);
        }

        [HttpGet]
        [Route("{apprenticeshipHashedId}/change-employer/confirm-employer", Name = RouteNames.ApprenticeConfirmEmployer)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        public async Task<IActionResult> ConfirmEmployer(ConfirmEmployerRequest request)
        {
            var viewModel = await _modelMapper.Map<ConfirmEmployerViewModel>(request);

            return View(viewModel);
        }

        [HttpPost]
        [Route("{apprenticeshipHashedId}/change-employer/confirm-employer", Name = RouteNames.ApprenticeConfirmEmployer)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        public IActionResult ConfirmEmployer(ConfirmEmployerViewModel viewModel)
        {
            if (viewModel.Confirm.Value)
            {
                return RedirectToAction("StartDate", new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId, viewModel.EmployerAccountLegalEntityPublicHashedId });
            }

            return RedirectToAction("SelectEmployer", new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId });
        }

        [HttpGet]
        [Route("{apprenticeshipHashedId}/change-employer/start-date", Name = RouteNames.ApprenticeStartDate)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        public async Task<IActionResult> StartDate(StartDateRequest request)
        {
            var viewModel = await _modelMapper.Map<StartDateViewModel>(request);

            return View(viewModel);
        }

        [HttpPost]
        [Route("{apprenticeshipHashedId}/change-employer/start-date", Name = RouteNames.ApprenticeStartDate)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        public async Task<IActionResult> StartDate(StartDateViewModel viewModel)
        {
            if (viewModel.InEditMode)
            {
                var request = await _modelMapper.Map<ConfirmRequest>(viewModel);
                return RedirectToAction(nameof(Confirm), request);

            }
            else
            {
                var request = await _modelMapper.Map<EndDateRequest>(viewModel);
                return RedirectToAction(nameof(EndDate), request);
            }
        }

        [HttpGet]
        [Route("{apprenticeshipHashedId}/change-employer/end-date", Name = RouteNames.ApprenticeEndDate)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        public async Task<IActionResult> EndDate(EndDateRequest request)
        {
            var viewModel = await _modelMapper.Map<EndDateViewModel>(request);

            return View(viewModel);
        }

        [HttpPost]
        [Route("{apprenticeshipHashedId}/change-employer/end-date", Name = RouteNames.ApprenticeEndDate)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        public async Task<IActionResult> EndDate(EndDateViewModel viewModel)
        {
            if (viewModel.InEditMode)
            {
                var request = await _modelMapper.Map<ConfirmRequest>(viewModel);
                return RedirectToAction(nameof(Confirm), request);

            }
            else
            {
                var request = await _modelMapper.Map<PriceRequest>(viewModel);
                return RedirectToAction(nameof(Price), request);
            }
        }

        [HttpGet]
        [Route("{apprenticeshipHashedId}/change-employer", Name = RouteNames.ChangeEmployer)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        public async Task<IActionResult> ChangeEmployer(ChangeEmployerRequest request)
        {
            var viewModel = await _modelMapper.Map<IChangeEmployerViewModel>(request);
            TempData["ChangeEmployerModel"] = JsonConvert.SerializeObject(viewModel);

            return RedirectToRoute(viewModel is InformViewModel ? 
                RouteNames.ChangeEmployerInform :
                RouteNames.ChangeEmployerDetails);
        }

        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        [Route("{apprenticeshipHashedId}/change-employer/change-employer-inform", Name = RouteNames.ChangeEmployerInform)]
        public IActionResult ChangeEmployerInform()
        {
            return View("Inform", JsonConvert.DeserializeObject<InformViewModel>(TempData["ChangeEmployerModel"].ToString()));
        }

        [Route("{apprenticeshipHashedId}/change-employer/change-employer-details", Name = RouteNames.ChangeEmployerDetails)]
        public IActionResult ChangeEmployerDetails()
        {
            return View("ChangeEmployerRequestDetails", JsonConvert.DeserializeObject<ChangeEmployerRequestDetailsViewModel>(TempData["ChangeEmployerModel"].ToString()));
        }

        [HttpGet]
        [Route("{apprenticeshipHashedId}/change-employer/select-employer", Name = RouteNames.ApprenticeSelectEmployer)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        public async Task<IActionResult> SelectEmployer(SelectEmployerRequest request)
        {
            var viewModel = await _modelMapper.Map<SelectEmployerViewModel>(request);

            return View(viewModel);
        }

        [HttpGet]
        [Route("{apprenticeshipHashedId}/change-employer/price", Name = RouteNames.ApprenticePrice)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        public async Task<IActionResult> Price(PriceRequest request)
        {
            var model = await _modelMapper.Map<PriceViewModel>(request);
            return View(model);
        }

        [HttpPost]
        [Route("{apprenticeshipHashedId}/change-employer/price", Name = RouteNames.ApprenticePrice)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        public async Task<IActionResult> Price(PriceViewModel viewModel)
        {
            var request = await _modelMapper.Map<ConfirmRequest>(viewModel);
            return RedirectToRoute(RouteNames.ApprenticeConfirm, request);
        }

        [HttpGet]
        [Route("{apprenticeshipHashedId}/change-employer/confirm", Name = RouteNames.ApprenticeConfirm)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        public async Task<IActionResult> Confirm(ConfirmRequest request)
        {
            var viewModel = await _modelMapper.Map<ConfirmViewModel>(request);
            return View(viewModel);
        }

        [HttpPost]
        [Route("{apprenticeshipHashedId}/change-employer/confirm", Name = RouteNames.ApprenticeConfirm)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        public async Task<IActionResult> Confirm(ConfirmViewModel viewModel)
        {
            var apiRequest = await _modelMapper.Map<CreateChangeOfPartyRequestRequest>(viewModel);
            await _commitmentsApiClient.CreateChangeOfPartyRequest(viewModel.ApprenticeshipId, apiRequest);
            TempData[nameof(ConfirmViewModel.NewEmployerName)] = viewModel.NewEmployerName;
            return RedirectToRoute(RouteNames.ApprenticeSent, new { viewModel.ApprenticeshipHashedId });
        }

        [HttpGet]
        [Route("{apprenticeshipHashedId}/change-employer/sent", Name = RouteNames.ApprenticeSent)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        public IActionResult Sent()
        {
            var model = TempData[nameof(ConfirmViewModel.NewEmployerName)] as string;
            return View(nameof(Sent), model);
        }

        [HttpGet]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        [Route("{apprenticeshipHashedId}/edit", Name = RouteNames.EditApprenticeship)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        public async Task<IActionResult> EditApprenticeship(EditApprenticeshipRequest request)
        {
            var viewModel = TempData.Get<EditApprenticeshipRequestViewModel>(ViewModelForEdit) ?? await _modelMapper.Map<EditApprenticeshipRequestViewModel>(request);
            return View(viewModel);
        }

        [HttpPost]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        [Route("{apprenticeshipHashedId}/edit")]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        public async Task<IActionResult> EditApprenticeship(string changeCourse, string changeDeliveryModel, EditApprenticeshipRequestViewModel viewModel)
        {
            if (changeCourse == "Edit" || changeDeliveryModel == "Edit")
            {
                TempData.Put(ViewModelForEdit, viewModel);
                return RedirectToAction(changeCourse == "Edit" ? nameof(SelectCourseForEdit) : nameof(SelectDeliveryModelForEdit), new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId });
            }

            var apprenticeship = await _commitmentsApiClient.GetApprenticeship(viewModel.ApprenticeshipId);
            var triggerCalculate = viewModel.CourseCode != apprenticeship.CourseCode ||
                (viewModel.CourseCode == apprenticeship.CourseCode && apprenticeship.StartDate <= viewModel.StartDate.Date.Value);

            if (triggerCalculate)
            {
                TrainingProgramme trainingProgramme;

                if (int.TryParse(viewModel.CourseCode, out var standardId))
                {
                    var standardVersionResponse = await _commitmentsApiClient.GetCalculatedTrainingProgrammeVersion(standardId, viewModel.StartDate.Date.Value);

                    trainingProgramme = standardVersionResponse.TrainingProgramme;
                }
                else
                {
                    var frameworkResponse = await _commitmentsApiClient.GetTrainingProgramme(viewModel.CourseCode);

                    trainingProgramme = frameworkResponse.TrainingProgramme;
                }

                viewModel.Version = trainingProgramme.Version;
                viewModel.TrainingName = trainingProgramme.Name;
                viewModel.HasOptions = trainingProgramme.Options.Any();
            }

            var validationRequest = await _modelMapper.Map<ValidateApprenticeshipForEditRequest>(viewModel);
            await _commitmentsApiClient.ValidateApprenticeshipForEdit(validationRequest);

            if(triggerCalculate)
            {
                viewModel.Option = null;
            }

            TempData.Put("EditApprenticeshipRequestViewModel", viewModel);

            if (viewModel.HasOptions)
            {
                return RedirectToAction("ChangeOption", new { apprenticeshipHashedId = viewModel.ApprenticeshipHashedId, providerId = viewModel.ProviderId });
            }

            return RedirectToAction("ConfirmEditApprenticeship", new { apprenticeshipHashedId = viewModel.ApprenticeshipHashedId, providerId = viewModel.ProviderId });
        }

        [HttpGet]
        [Route("{apprenticeshipHashedId}/edit/select-course")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SelectCourseForEdit(EditApprenticeshipRequest request)
        {
            var draft = TempData.GetButDontRemove<EditApprenticeshipRequestViewModel>(ViewModelForEdit);
            var model = await _modelMapper.Map<SelectCourseViewModel>(draft);
            return View("SelectCourse", model);
        }

        [HttpPost]
        [Route("{apprenticeshipHashedId}/edit/select-course")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public IActionResult SetCourseForEdit(SelectCourseViewModel model)
        {
            if (string.IsNullOrEmpty(model.CourseCode))
            {
                throw new CommitmentsApiModelException(new List<ErrorDetail>
                    {new ErrorDetail(nameof(model.CourseCode), "You must select a training course")});
            }

            var draft = TempData.GetButDontRemove<EditApprenticeshipRequestViewModel>(ViewModelForEdit);
            draft.CourseCode = model.CourseCode;

            TempData.Put(ViewModelForEdit, draft);

            return RedirectToAction(nameof(SelectDeliveryModelForEdit), new { model.ProviderId, model.ApprenticeshipHashedId});
        }

        [HttpGet]
        [Route("{apprenticeshipHashedId}/edit/select-delivery-model")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SelectDeliveryModelForEdit(EditApprenticeshipRequest request)
        {
            var draft = TempData.GetButDontRemove<EditApprenticeshipRequestViewModel>(ViewModelForEdit);
            var model = await _modelMapper.Map<SelectDeliveryModelViewModel>(draft);

            if (model.DeliveryModels.Length > 1)
            {
                return View("SelectDeliveryModel", model);
            }
            draft.DeliveryModel = model.DeliveryModels.FirstOrDefault();
            TempData.Put(ViewModelForEdit, draft);

            return RedirectToAction("EditApprenticeship", request);
        }

        [HttpPost]
        [Route("{DraftApprenticeshipHashedId}/edit/select-delivery-model")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public IActionResult SetDeliveryModelForEdit(SelectDeliveryModelViewModel model)
        {
            if (model.DeliveryModel == null)
            {
                throw new CommitmentsApiModelException(new List<ErrorDetail>
                    {new ErrorDetail("DeliveryModel", "You must select the apprenticeship delivery model")});
            }

            var draft = TempData.GetButDontRemove<EditApprenticeshipRequestViewModel>(ViewModelForEdit);
            draft.DeliveryModel = model.DeliveryModel.Value;
            TempData.Put(ViewModelForEdit, draft);
            return RedirectToAction("EditApprenticeship", new { draft.ProviderId, draft.ApprenticeshipHashedId });
        }

        [HttpGet]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        [Route("{apprenticeshipHashedId}/edit/change-version", Name = RouteNames.ChangeVersion)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        public async Task<IActionResult> ChangeVersion(ChangeVersionRequest request)
        {
            var viewModel = await _modelMapper.Map<ChangeVersionViewModel>(request);

            var editViewModel = TempData.GetButDontRemove<EditApprenticeshipRequestViewModel>("EditApprenticeshipRequestViewModel");

            if (editViewModel != null)
            {
                viewModel.SelectedVersion = editViewModel.Version;
            }

            return View(viewModel);
        }

        [HttpPost]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        [Route("{apprenticeshipHashedId}/edit/change-version", Name = RouteNames.ChangeVersion)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        public async Task<IActionResult> ChangeVersion(ChangeVersionViewModel viewModel)
        {
            var editApprenticeshipRequestViewModel = await _modelMapper.Map<EditApprenticeshipRequestViewModel>(viewModel);

            TempData.Put("EditApprenticeshipRequestViewModel", editApprenticeshipRequestViewModel);

            if (editApprenticeshipRequestViewModel.HasOptions)
            {
                return RedirectToAction("ChangeOption", new { apprenticeshipHashedId = viewModel.ApprenticeshipHashedId, providerId = viewModel.ProviderId});
            }

            return RedirectToAction("ConfirmEditApprenticeship", new { apprenticeshipHashedId = viewModel.ApprenticeshipHashedId, providerId = viewModel.ProviderId });
        }

        [HttpGet]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        [Route("{apprenticeshipHashedId}/edit/change-option", Name = RouteNames.ChangeOption)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        public async Task<IActionResult> ChangeOption(ChangeOptionRequest request)
        {
            var viewModel = await _modelMapper.Map<ChangeOptionViewModel>(request);

            return View(viewModel);
        }

        [HttpPost]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        [Route("{apprenticeshipHashedId}/edit/change-option")]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        public async Task<IActionResult> ChangeOption(ChangeOptionViewModel viewModel)
        {
            var editViewModel = await _modelMapper.Map<EditApprenticeshipRequestViewModel>(viewModel);

            TempData.Put("EditApprenticeshipRequestViewModel", editViewModel);

            return RedirectToAction("ConfirmEditApprenticeship", new { apprenticeshipHashedId = viewModel.ApprenticeshipHashedId, providerId = viewModel.ProviderId });
        }

        [HttpGet]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        [Route("{apprenticeshipHashedId}/edit/confirm")]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        public async Task<IActionResult> ConfirmEditApprenticeship()
        {
            var editApprenticeshipRequestViewModel = TempData.GetButDontRemove<EditApprenticeshipRequestViewModel>("EditApprenticeshipRequestViewModel");
            var viewModel = await _modelMapper.Map<ConfirmEditApprenticeshipViewModel>(editApprenticeshipRequestViewModel);

            return View(viewModel);
        }

        [HttpPost]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        [Route("{apprenticeshipHashedId}/edit/confirm")]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        public async Task<IActionResult> ConfirmEditApprenticeship(ConfirmEditApprenticeshipViewModel viewModel)
        {
            if (viewModel.ConfirmChanges.Value)
            {
                var request = await _modelMapper.Map<EditApprenticeshipApiRequest>(viewModel);
                var result = await _commitmentsApiClient.EditApprenticeship(request);

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
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        [Route("{apprenticeshipHashedId}/datalock/requestrestart", Name = RouteNames.RequestRestart)]
        public async Task<IActionResult> DataLockRequestRestart(DataLockRequestRestartRequest request)
        {
            var viewModel = await _modelMapper.Map<DataLockRequestRestartViewModel>(request);

            return View(viewModel);
        }

        [HttpPost]        
        [Route("{apprenticeshipHashedId}/datalock/requestrestart", Name = RouteNames.RequestRestart)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        public IActionResult DataLockRequestRestart(DataLockRequestRestartViewModel viewModel)
        {
            if (viewModel.SubmitStatusViewModel.HasValue && viewModel.SubmitStatusViewModel.Value == SubmitStatusViewModel.Confirm)
            {
                return RedirectToAction( "ConfirmRestart", new DatalockConfirmRestartRequest { ApprenticeshipHashedId=viewModel.ApprenticeshipHashedId, ProviderId = viewModel.ProviderId });
            }          

            return RedirectToAction("Details", "Apprentice", new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId });
        }

        [HttpGet]        
        [Route("{apprenticeshipHashedId}/datalock/confirmrestart", Name = RouteNames.ConfirmRestart)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        public IActionResult ConfirmRestart(DatalockConfirmRestartRequest request)
        {            
            var viewModel = new DatalockConfirmRestartViewModel { ApprenticeshipHashedId = request.ApprenticeshipHashedId, ProviderId = request.ProviderId };
            return View("DataLockConfirmRestart", viewModel);
        }

        [HttpPost]        
        [Route("{apprenticeshipHashedId}/datalock/confirmrestart", Name = RouteNames.ConfirmRestart)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        public async Task<ActionResult> ConfirmRestart(DatalockConfirmRestartViewModel viewModel)
        {            
            if (viewModel.SendRequestToEmployer.HasValue && viewModel.SendRequestToEmployer.Value)
            {
                var request = await _modelMapper.Map<TriageDataLocksRequest>(viewModel);
                await _commitmentsApiClient.TriageDataLocks(viewModel.ApprenticeshipId, request);
            }

            return RedirectToAction("Details", "Apprentice", new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId });
        }

        [HttpGet]
        [Route("{apprenticeshipHashedId}/datalock", Name = RouteNames.UpdateDateLock)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        public async Task<IActionResult> UpdateDataLock(UpdateDateLockRequest request)
        {
            var viewModel = await _modelMapper.Map<UpdateDateLockViewModel>(request);
            return View("UpdateDataLock", viewModel);
        }

        [HttpPost]
        [Route("{apprenticeshipHashedId}/datalock", Name = RouteNames.UpdateDateLock)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        public IActionResult UpdateDataLock(UpdateDateLockViewModel viewModel)
        {
            if (viewModel.SubmitStatusViewModel == SubmitStatusViewModel.Confirm)
            {
                return RedirectToAction("ConfirmDataLockChanges", new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId });
            }

            return RedirectToAction("Details", "Apprentice", new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId });
        }

        [HttpGet]
        [Route("{apprenticeshipHashedId}/datalock/confirm", Name = RouteNames.UpdateDataLockConfirm)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        public async Task<ActionResult> ConfirmDataLockChanges(ConfirmDataLockChangesRequest request)
        {
            var viewModel = await _modelMapper.Map<ConfirmDataLockChangesViewModel>(request);
            return View(viewModel);
        }

        [HttpPost]
        [Route("{apprenticeshipHashedId}/datalock/confirm", Name = RouteNames.UpdateDataLockConfirm)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        public async Task<IActionResult> ConfirmDataLockChanges(ConfirmDataLockChangesViewModel viewModel)
        {
            if (viewModel.SubmitStatusViewModel != null && viewModel.SubmitStatusViewModel.Value == SubmitStatusViewModel.Confirm)
            {
                var request = await _modelMapper.Map<TriageDataLocksRequest>(viewModel);
                await _commitmentsApiClient.TriageDataLocks(viewModel.ApprenticeshipId, request);
            }

            return RedirectToAction("Details", "Apprentice", new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId });
        }

        [Route("{apprenticeshipHashedId}/details/resend-email-invitation")]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship)]
        [HttpGet]
        public async Task<IActionResult> ResendEmailInvitation([FromServices] IAuthenticationService authenticationService, ResendEmailInvitationRequest request)
        {
            try
            {
                await _commitmentsApiClient.ResendApprenticeshipInvitation(request.ApprenticeshipId, new SaveDataRequest { UserInfo = authenticationService.UserInfo });

                TempData.AddFlashMessage("The invitation email has been resent.", null, ITempDataDictionaryExtensions.FlashMessageLevel.Success);
            }
            catch { }            

            return RedirectToAction("Details", new
            {
                ProviderId = request.ProviderId,
                ApprenticeshipHashedId = request.ApprenticeshipHashedId
            });
        }
    }
}