using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}")]
    public class ProviderAccountController : Controller
    {
        private readonly ProviderSharedUIConfiguration _providerSharedUiConfiguration;

        public ProviderAccountController(ProviderSharedUIConfiguration providerSharedUiConfiguration)
        {
            _providerSharedUiConfiguration = providerSharedUiConfiguration;
        }

        [Route("signout", Name = RouteNames.ProviderSignOut)]
        public IActionResult SignOut()
        {
            return SignOut(
                new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                {
                    RedirectUri = $"{_providerSharedUiConfiguration.DashboardUrl}/account",
                    AllowRefresh = true
                },
                CookieAuthenticationDefaults.AuthenticationScheme,
                WsFederationDefaults.AuthenticationScheme);
        }
    }
}
