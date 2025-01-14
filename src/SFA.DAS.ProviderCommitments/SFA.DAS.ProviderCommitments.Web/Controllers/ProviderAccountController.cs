using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    public class ProviderAccountController(ProviderSharedUIConfiguration providerSharedUiConfiguration)
        : Controller
    {
        [AllowAnonymous]
        [Route("signout", Name = RouteNames.ProviderSignOut)]
        public IActionResult SignOut()
        {
            return SignOut(
                new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                {
                    AllowRefresh = true
                },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        [Route("{providerId}/dashboard")]
        public IActionResult Dashboard()
        {
            return RedirectPermanent(providerSharedUiConfiguration.DashboardUrl);
        }
    }
}