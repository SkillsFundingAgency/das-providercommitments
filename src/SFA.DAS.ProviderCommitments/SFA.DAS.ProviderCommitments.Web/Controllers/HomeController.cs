using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    public class HomeController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
