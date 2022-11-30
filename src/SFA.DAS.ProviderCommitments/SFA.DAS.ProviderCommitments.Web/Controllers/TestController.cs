using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index([FromServices] IOuterApiClient outerApiClient)
        {
            var result = outerApiClient.Get<string>(new TestApimRole());
            return Ok(result);
        }
    }

    public class TestApimRole : IGetApiRequest
    {
        public string GetUrl => $"Test/Call/Commitment";
    }
}
