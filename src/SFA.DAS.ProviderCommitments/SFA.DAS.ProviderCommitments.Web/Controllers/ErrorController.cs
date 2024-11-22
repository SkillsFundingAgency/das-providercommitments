using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Provider.Shared.UI.Attributes;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Error;

namespace SFA.DAS.ProviderCommitments.Web.Controllers;

[AllowAnonymous]
[HideNavigationBar(hideAccountHeader: false, hideNavigationLinks: true)]
public class ErrorController(IConfiguration configuration) : Controller
{
    [Route("error/error")]
    [Route("error/{statuscode?}")]
    public IActionResult Error(int? statusCode, bool isPostRequest = false)
    {       
        var useDfESignIn = configuration.UseDfeSignIn();

        return statusCode switch
        {
            403 => View("403", new Error403ViewModel(
                configuration["ResourceEnvironmentName"])
            { UseDfESignIn = useDfESignIn, IsPostRequest = isPostRequest }),
            404 => View(statusCode.ToString()),
            _ => View()
        };
    }
}