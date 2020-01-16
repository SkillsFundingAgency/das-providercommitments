using Microsoft.AspNetCore.Mvc;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}/apprentices")]
    public class ApprenticeController : Controller
    {
        private readonly IModelMapper _modelMapper;
     
        public ApprenticeController(IModelMapper modelMapper)
        {
            _modelMapper = modelMapper;
        }

        [Route("{apprenticeshipHashedId}")]
        public async Task<IActionResult> Details(DetailsRequest request)
        {
            var viewModel = await _modelMapper.Map<DetailsViewModel>(request);
            return View(viewModel);
        }
    }
}