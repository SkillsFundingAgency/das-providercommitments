using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.CommitmentPermissions.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Attributes;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Web.Cookies;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using System;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}/apprentices")]
    [SetNavigationSection(NavigationSection.ManageApprentices)]
    public class ApprenticeController : Controller
    {
        private readonly ICookieStorageService<IndexRequest> _cookieStorage;
        private readonly IModelMapper _modelMapper;
        private readonly ICommitmentsApiClient _commitmentApiClient;

        public ApprenticeController(IModelMapper modelMapper, ICookieStorageService<IndexRequest> cookieStorage, ICommitmentsApiClient commitmentApiClient)
        {
            _modelMapper = modelMapper;
            _cookieStorage = cookieStorage;
            _commitmentApiClient = commitmentApiClient;
        }

        [HttpGet]
        [Route("{apprenticeshipHashedId}/change-employer/confirm-employer", Name = RouteNames.ApprenticeConfirmEmployer)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship, ProviderFeature.ChangeOfEmployer)]
        public async Task<IActionResult> ConfirmEmployer(ConfirmEmployerRequest request)
        {
            var viewModel = await _modelMapper.Map<ConfirmEmployerViewModel>(request);

            return View(viewModel);
        }

        [HttpPost]
        [Route("{apprenticeshipHashedId}/change-employer/confirm-employer", Name = RouteNames.ApprenticeConfirmEmployer)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship, ProviderFeature.ChangeOfEmployer)]
        public IActionResult ConfirmEmployer(ConfirmEmployerViewModel viewModel)
        {
            if (viewModel.Confirm.Value)
            {
                return RedirectToAction("Dates", new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId, viewModel.EmployerAccountLegalEntityPublicHashedId });
            }

            return RedirectToAction("SelectEmployer", new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId });
        }

        [HttpGet]
        [Route("{apprenticeshipHashedId}/change-employer/dates", Name = RouteNames.ApprenticeDates)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship, ProviderFeature.ChangeOfEmployer)]
        public async Task<IActionResult> Dates(DatesRequest request)
        {
            var viewModel = await _modelMapper.Map<DatesViewModel>(request);

            return View(viewModel);
        }

        [HttpPost]
        [Route("{apprenticeshipHashedId}/change-employer/dates", Name = RouteNames.ApprenticeDates)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship, ProviderFeature.ChangeOfEmployer)]
        public async Task<IActionResult> Dates(DatesViewModel viewModel)
        {
            if (viewModel.InEditMode)
            {
                var request = await _modelMapper.Map<ConfirmRequest>(viewModel);
                return RedirectToAction(nameof(Confirm), request);

            }
            else
            {
                var request = await _modelMapper.Map<PriceRequest>(viewModel);
                return RedirectToAction(nameof(Price), new { viewModel.ProviderId, viewModel.ApprenticeshipHashedId, viewModel.EmployerAccountLegalEntityPublicHashedId, StartDate = viewModel.StartDate.MonthYear });
            }
        }

        [Route("{apprenticeshipHashedId}", Name = RouteNames.ApprenticeDetail)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship, ProviderFeature.ApprenticeDetailsV2)]
        public async Task<IActionResult> Details(DetailsRequest request)
        {
            var viewModel = await _modelMapper.Map<DetailsViewModel>(request);
            return View(viewModel);
        }

        [HttpGet]
        [Route("download", Name = RouteNames.DownloadApprentices)]
        [DasAuthorize(ProviderFeature.ManageApprenticesV2)]
        public async Task<IActionResult> Download(DownloadRequest request)
        {
            var downloadViewModel = await _modelMapper.Map<DownloadViewModel>(request);
            return File(downloadViewModel.Content, downloadViewModel.ContentType, downloadViewModel.Name);
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

        [HttpGet]
        [Route("{apprenticeshipHashedId}/change-employer", Name = RouteNames.ApprenticeInform)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship, ProviderFeature.ChangeOfEmployer)]
        public async Task<IActionResult> Inform(InformRequest request)
        {
            var viewModel = await _modelMapper.Map<InformViewModel>(request);

            return View(viewModel);
        }

        [HttpGet]
        [Route("{apprenticeshipHashedId}/change-employer/select-employer", Name = RouteNames.ApprenticeSelectEmployer)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship, ProviderFeature.ChangeOfEmployer)]
        public async Task<IActionResult> SelectEmployer(SelectEmployerRequest request)
        {
            var viewModel = await _modelMapper.Map<SelectEmployerViewModel>(request);

            return View(viewModel);
        }

        [HttpGet]
        [Route("{apprenticeshipHashedId}/change-employer/price", Name = RouteNames.ApprenticePrice)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship, ProviderFeature.ChangeOfEmployer)]
        public async Task<IActionResult> Price(PriceRequest request)
        {
            var model = await _modelMapper.Map<PriceViewModel>(request);
            return View(model);
        }

        [HttpPost]
        [Route("{apprenticeshipHashedId}/change-employer/price", Name = RouteNames.ApprenticePrice)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship, ProviderFeature.ChangeOfEmployer)]
        public async Task<IActionResult> Price(PriceViewModel viewModel)
        {
            var request = await _modelMapper.Map<ConfirmRequest>(viewModel);
            return RedirectToRoute(RouteNames.ApprenticeConfirm, new { request.ProviderId, request.ApprenticeshipHashedId, request.EmployerAccountLegalEntityPublicHashedId, request.StartDate, request.Price });
        }

        [HttpGet]
        [Route("{apprenticeshipHashedId}/change-employer/confirm", Name = RouteNames.ApprenticeConfirm)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship, ProviderFeature.ChangeOfEmployer)]
        public async Task<IActionResult> Confirm(ConfirmRequest request)
        {
            var viewModel = await _modelMapper.Map<ConfirmViewModel>(request);
            return View(viewModel);
        }

        [HttpPost]
        [Route("{apprenticeshipHashedId}/change-employer/confirm", Name = RouteNames.ApprenticeConfirm)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship, ProviderFeature.ChangeOfEmployer)]
        public async Task<IActionResult> Confirm(ConfirmViewModel viewModel)
        {
            var apiRequest = await _modelMapper.Map<CreateChangeOfPartyRequestRequest>(viewModel);
            await _commitmentApiClient.CreateChangeOfPartyRequest(viewModel.ApprenticeshipId, apiRequest);
            return RedirectToRoute(RouteNames.ApprenticeSent, new { viewModel.ApprenticeshipHashedId });
        }

        [HttpGet]
        [Route("{apprenticeshipHashedId}/change-employer/sent", Name = RouteNames.ApprenticeSent)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship, ProviderFeature.ChangeOfEmployer)]
        public IActionResult Sent()
        {
            return View();
        }
    }
}