using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("v2/{providerId}/apprentices")]
    public class ManageApprenticesController : Controller
    {
        private readonly ICommitmentsService _commitmentsService;

        public ManageApprenticesController(ICommitmentsService commitmentsService)
        {
            _commitmentsService = commitmentsService;
        }

        [Route("all")]
        public async Task<IActionResult> Index(uint providerId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new ManageApprenticesViewModel
            {
                ProviderId = providerId,
                Apprenticeships = await _commitmentsService.GetApprovedApprenticeships(providerId)
            };

            return View(model);
        }
    }
}