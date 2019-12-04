using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}/apprentices/manageV2")]
    public class ManageApprenticesController : Controller
    {
        public IActionResult Index(long providerId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new ManageApprenticesViewModel {ProviderId = providerId};

            return View(model);
        }
    }
}