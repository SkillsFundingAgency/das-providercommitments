using Microsoft.AspNetCore.Mvc;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using System.Threading.Tasks;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Attributes;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.Authorization.CommitmentPermissions.Options;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}/apprentices")]
    [SetNavigationSection(NavigationSection.ManageApprentices)]
    public class ApprenticeController : Controller
    {
        private readonly IModelMapper _modelMapper;
        private ICurrentDateTime _currentDateTime;

        public ApprenticeController(IModelMapper modelMapper,
            ICurrentDateTime currentDateTime)
        {
            _modelMapper = modelMapper;
            _currentDateTime = currentDateTime;
        }

        [Route("", Name = RouteNames.ManageApprentices)]
        public async Task<IActionResult> Index(long providerId, ManageApprenticesFilterModel filterModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var request = new GetApprenticeshipsRequest
            {
                ProviderId = providerId,
                PageNumber = filterModel.PageNumber,
                PageItemCount = ProviderCommitmentsWebConstants.NumberOfApprenticesPerSearchPage,
                SortField = filterModel.SortField,
                ReverseSort = filterModel.ReverseSort,
                SearchTerm = filterModel.SearchTerm,
                SelectedEmployer = filterModel.SelectedEmployer,
                SelectedCourse = filterModel.SelectedCourse,
                SelectedStatus = filterModel.SelectedStatus,
                SelectedStartDate = filterModel.SelectedStartDate,
                SelectedEndDate = filterModel.SelectedEndDate
            };

            var viewModel = await _modelMapper.Map<ManageApprenticesViewModel>(request);
            viewModel.SortedByHeader();

            return View(viewModel);
        }

        [Route("{apprenticeshipHashedId}")]
        [DasAuthorize(CommitmentOperation.AccessApprenticeship, ProviderFeature.ApprenticeDetailsV2)]
        public async Task<IActionResult> Details(DetailsRequest request)
        {
            var viewModel = await _modelMapper.Map<DetailsViewModel>(request);
            return View(viewModel);
        }



        [HttpGet]
        [Route("download", Name = RouteNames.DownloadApprentices)]
        public async Task<IActionResult> Download(long providerId, ManageApprenticesFilterModel filterModel)
        {
            var request = new GetApprenticeshipsCsvContentRequest
            {
                ProviderId = providerId,
                FilterModel = filterModel
            };

            var csvFileContent = await _modelMapper.Map<byte[]>(request);

            return File(csvFileContent, "text/csv", $"{"Manageyourapprentices"}_{_currentDateTime.Now:yyyyMMddhhmmss}.csv");
        }
    }
}