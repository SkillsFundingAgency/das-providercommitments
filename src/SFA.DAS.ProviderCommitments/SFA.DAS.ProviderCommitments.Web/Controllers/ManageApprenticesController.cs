using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Authorization.ProviderPermissions.Options;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Attributes;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using SFA.DAS.ProviderCommitments.Features;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}/apprentices")]
    [DasAuthorize(ProviderFeature.ManageApprenticesV2)]
    public class ManageApprenticesController : Controller
    {
        private readonly IMapper<GetApprenticeshipsRequest, ManageApprenticesViewModel> _apprenticeshipMapper;
        private readonly IMapper<GetApprenticeshipsCsvContentRequest, byte[]> _csvMapper;

        public ManageApprenticesController(IMapper<GetApprenticeshipsRequest,ManageApprenticesViewModel> apprenticeshipMapper, IMapper<GetApprenticeshipsCsvContentRequest,byte[]> csvMapper)
        {
            _apprenticeshipMapper = apprenticeshipMapper;
            _csvMapper = csvMapper;
        }

        [Route("", Name = RouteNames.ManageApprentices)]
        [SetNavigationSection(NavigationSection.ManageApprentices)]
        public async Task<IActionResult> Index(long providerId, int pageNumber = 1)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var request = new GetApprenticeshipsRequest
            {
                ProviderId = providerId,
                PageNumber = pageNumber,
                PageItemCount = ProviderCommitmentsWebConstants.NumberOfApprenticesPerSearchPage
            };

            var viewModel = await _apprenticeshipMapper.Map(request);

            return View(viewModel);
        }

        [HttpGet]
        [Route("download", Name = RouteNames.DownloadApprentices)]
        public async Task<IActionResult> Download(long providerId)
        {
            var request = new GetApprenticeshipsCsvContentRequest{ProviderId = providerId};

            var csvFileContent = await _csvMapper.Map(request);

            return File(csvFileContent, "text/csv", $"{"Manageyourapprentices"}_{DateTime.Now:yyyyMMddhhmmss}.csv");
        }
    }
}