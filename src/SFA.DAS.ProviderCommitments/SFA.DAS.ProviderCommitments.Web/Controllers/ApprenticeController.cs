using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFA.DAS.Authorization.CommitmentPermissions.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Attributes;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Cookies;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using System.Threading.Tasks;

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

        public ApprenticeController(IModelMapper modelMapper, ICookieStorageService<IndexRequest> cookieStorage, ICommitmentsApiClient commitmentsApiClient)
        {
            _modelMapper = modelMapper;
            _cookieStorage = cookieStorage;
            _commitmentsApiClient = commitmentsApiClient;
        }

        [Route("", Name = RouteNames.ApprenticesIndex)]
        [DasAuthorize(ProviderFeature.ManageApprenticesV2)]
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
        [DasAuthorize(CommitmentOperation.AccessApprenticeship, ProviderFeature.ApprenticeDetailsV2)]
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
        public async Task<IActionResult> ViewApprenticeshipUpdates(ViewApprenticeshipUpdatesViewModel viewModel)
        {
            if (viewModel.UndoChanges.Value)
            {
                var request = new UndoApprenticeshipUpdatesRequest
                {
                    ApprenticeshipId = viewModel.ApprenticeshipId,
                    ProviderId = viewModel.ProviderId
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
        public async Task<IActionResult> ReviewApprenticeshipUpdates(ReviewApprenticeshipUpdatesViewModel viewModel)
        {
            if (viewModel.AcceptChanges.Value)
            {
                var request = new AcceptApprenticeshipUpdatesRequest
                {
                    ApprenticeshipId = viewModel.ApprenticeshipId,
                    ProviderId = viewModel.ProviderId
                };

                await _commitmentsApiClient.AcceptApprenticeshipUpdates(viewModel.ApprenticeshipId, request);

                TempData.AddFlashMessage(ChangesApprovedFlashMessage, ITempDataDictionaryExtensions.FlashMessageLevel.Success);
            }
            else if (!viewModel.AcceptChanges.Value)
            {
                var request = new RejectApprenticeshipUpdatesRequest
                {
                    ApprenticeshipId = viewModel.ApprenticeshipId,
                    ProviderId = viewModel.ProviderId
                };

                await _commitmentsApiClient.RejectApprenticeshipUpdates(viewModel.ApprenticeshipId, request);

                TempData.AddFlashMessage(ChangesRejectedFlashMessage, ITempDataDictionaryExtensions.FlashMessageLevel.Success);
            }

            return RedirectToRoute(RouteNames.ApprenticeDetail, new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId });
        }

        [HttpGet]
        [Route("download", Name = RouteNames.DownloadApprentices)]
        [DasAuthorize(ProviderFeature.ManageApprenticesV2)]
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
                return RedirectToAction(nameof(EndDate), new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId, viewModel.EmployerAccountLegalEntityPublicHashedId, StartDate = viewModel.StartDate.MonthYear });
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
                return RedirectToAction(nameof(Price), new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId, viewModel.EmployerAccountLegalEntityPublicHashedId, StartDate = viewModel.StartDate, EndDate = viewModel.EndDate.MonthYear });
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
            return RedirectToRoute(RouteNames.ApprenticeConfirm, new { request.ProviderId, request.ApprenticeshipHashedId, request.EmployerAccountLegalEntityPublicHashedId, request.StartDate, request.EndDate, request.Price });
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
        [Route("{apprenticeshipHashedId}/datalock/requestrestart", Name = RouteNames.RequestRestart)]
        public async Task<IActionResult> DataLockRequestRestart(DataLockRequestRestartRequest request)
        {
            var viewModel = await _modelMapper.Map<DataLockRequestRestartViewModel>(request);

            return View(viewModel);
        }

        [HttpPost]        
        [Route("{apprenticeshipHashedId}/datalock/requestrestart", Name = RouteNames.RequestRestart)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
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
        public IActionResult ConfirmRestart(DatalockConfirmRestartRequest request)
        {            
            var viewModel = new DatalockConfirmRestartViewModel { ApprenticeshipHashedId = request.ApprenticeshipHashedId, ProviderId = request.ProviderId };
            return View("DataLockConfirmRestart", viewModel);
        }

        [HttpPost]        
        [Route("{apprenticeshipHashedId}/datalock/confirmrestart", Name = RouteNames.ConfirmRestart)]
        [Authorize(Policy = nameof(PolicyNames.HasAccountOwnerPermission))]
        public async Task<IActionResult> ConfirmRestart(DatalockConfirmRestartViewModel viewModel)
        {            
            if (viewModel.SendRequestToEmployer.HasValue && viewModel.SendRequestToEmployer.Value)
            {              
                await _commitmentsApiClient.TriageDataLocks(viewModel.ApprenticeshipId, new TriageDataLocksRequest { TriageStatus = CommitmentsV2.Types.TriageStatus.Restart });                
            }

            return RedirectToAction("Details", "Apprentice", new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId });
        }
    }
}