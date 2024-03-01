using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Provider.Shared.UI.Attributes;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Error;

namespace SFA.DAS.ProviderCommitments.Web.Controllers;

[AllowAnonymous]
[HideNavigationBar(hideAccountHeader: false, hideNavigationLinks: true)]
public class ErrorController : Controller
{
    private readonly IConfiguration _configuration;
    public ErrorController(IConfiguration configuration) => _configuration = configuration;

    [Route("error/error")]
    [Route("error/{statuscode?}")]
    public IActionResult Error(int? statusCode)
    {
        var useDfESignIn = _configuration.UseDfeSignIn();

        return statusCode switch
        {
            403 => View("403", new Error403ViewModel(_configuration["ResourceEnvironmentName"]) { UseDfESignIn = useDfESignIn }),
            404 => View(statusCode.ToString()),
            _ => View()
        };
    }
}