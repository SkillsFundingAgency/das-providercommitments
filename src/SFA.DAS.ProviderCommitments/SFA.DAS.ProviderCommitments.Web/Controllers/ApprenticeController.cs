using Microsoft.AspNetCore.Mvc;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using System.Threading.Tasks;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Attributes;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.Authorization.CommitmentPermissions.Options;
using SFA.DAS.CommitmentsV2.Shared.ActionResults;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}/apprentices")]
    [SetNavigationSection(NavigationSection.ManageApprentices)]
    public class ApprenticeController : Controller
    {
        private readonly IModelMapper _modelMapper;
        private readonly ILogger<ApprenticeController> _logger;

        public ApprenticeController(
            IModelMapper modelMapper,
            ILogger<ApprenticeController> logger)
        {
            _modelMapper = modelMapper;
            _logger = logger;
        }

        [Route("", Name = RouteNames.ApprenticesIndex)]
        [DasAuthorize(ProviderFeature.ManageApprenticesV2)]
        public async Task<IActionResult> Index(IndexRequest request)
        {   
            var viewModel = await _modelMapper.Map<IndexViewModel>(request);
            viewModel.SortedByHeader();

            return View(viewModel);
        }

        [Route("{apprenticeshipHashedId}", Name= RouteNames.ApprenticeDetail)]
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
            
            return new FileCallbackResult(downloadViewModel.ContentType, async (outputStream, _) =>
            {
                var moreData = true;
                while (moreData)
                {
                    _logger.LogDebug($"Streaming page number: [{downloadViewModel.Request.PageNumber}]");

                    var stream2 = await downloadViewModel.GetAndCreateContent(downloadViewModel.Request);
                    
                    if (stream2.Length == 0)
                    {
                        moreData = false;
                    }

                    downloadViewModel.Request.PageNumber += 1;

                    stream2.CopyTo(outputStream);

                    _logger.LogDebug($"Page number: [{downloadViewModel.Request.PageNumber}] has been copied to output stream");
                
                    downloadViewModel.Dispose();
                }

                _logger.LogDebug("Finished streaming all pages");
                
            }){FileDownloadName = downloadViewModel.Name};
            
        }
    }
}