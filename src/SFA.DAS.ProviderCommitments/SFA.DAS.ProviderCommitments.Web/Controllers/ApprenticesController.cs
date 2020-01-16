using Microsoft.AspNetCore.Mvc;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}/apprentices")]
    public class ApprenticesController : Controller
    {
        private readonly IModelMapper _modelMapper;
     
        public ApprenticesController(IModelMapper modelMapper)
        {
            _modelMapper = modelMapper;
        }

        [Route("{apprenticeshipHashedId}")]
        public async Task<IActionResult> Details(ApprenticeDetailsRequest request)
        {
            var viewModel = await _modelMapper.Map<ApprenticeDetailsViewModel>(request);
            return View(viewModel);
        }
    }
}