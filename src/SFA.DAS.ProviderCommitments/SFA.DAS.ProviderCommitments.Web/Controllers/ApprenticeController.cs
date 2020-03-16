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

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}/apprentices")]
    [SetNavigationSection(NavigationSection.ManageApprentices)]
    public class ApprenticeController : Controller
    {
        private readonly ICookieStorageService<IndexRequest> _cookieStorage;
        private readonly IModelMapper _modelMapper;
        public ApprenticeController(IModelMapper modelMapper, ICookieStorageService<IndexRequest> cookieStorage)
        {
            _modelMapper = modelMapper;
            _cookieStorage = cookieStorage;
        }

        [HttpGet]
        [Route("{apprenticeshipHashedId}/change-employer/price", Name = RouteNames.ApprenticeChangePrice)]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship, ProviderFeature.ChangeOfEmployer)]
        public Task<IActionResult> ChangePrice(PriceRequest request)
        {
            throw new NotImplementedException();
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
            var request = await _modelMapper.Map<PriceRequest>(viewModel);

            return RedirectToAction(nameof(ChangePrice), request);
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
    }
}