using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}/apprentices")]
    public class ManageApprenticesController : Controller
    {
        private readonly IMapper<GetApprenticeshipsRequest, ManageApprenticesViewModel> _apprenticeshipMapper;
        private readonly IMapper<GetApprenticeshipsCsvContentRequest, byte[]> _csvMapper;
        private readonly ICurrentDateTime _currentDateTime;

        public ManageApprenticesController(
            IMapper<GetApprenticeshipsRequest,ManageApprenticesViewModel> apprenticeshipMapper, 
            IMapper<GetApprenticeshipsCsvContentRequest,byte[]> csvMapper,
            ICurrentDateTime currentDateTime)
        {
            _apprenticeshipMapper = apprenticeshipMapper;
            _csvMapper = csvMapper;
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
            var viewModel = await _apprenticeshipMapper.Map(request);
            
            viewModel.SortedByHeader();

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

            var csvFileContent = await _csvMapper.Map(request);

            return File(csvFileContent, "text/csv", $"{"Manageyourapprentices"}_{_currentDateTime.Now:yyyyMMddhhmmss}.csv");
        }

        [Route("{apprenticeshipId}", Name = "ApprenticeshipDetails")]
        public IActionResult Details(uint providerId, long apprenticeshipId)
        {
            return Content($"Details of apprenticeship Id:[{apprenticeshipId}].");
        }
    }
}