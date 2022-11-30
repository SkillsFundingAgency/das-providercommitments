using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}/[controller]")]
    public class TestController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index([FromServices] IOuterApiClient outerApiClient)
        {
            var result = await outerApiClient.Get<ResponseRole>(new TestApimRole());
            return Ok(result);
        }
    }

    public class ResponseRole
    {
        public string Role { get; set; }
    }

    public class TestApimRole : IGetApiRequest
    {
        public string GetUrl => $"Test/Call/Commitment";
    }
}
