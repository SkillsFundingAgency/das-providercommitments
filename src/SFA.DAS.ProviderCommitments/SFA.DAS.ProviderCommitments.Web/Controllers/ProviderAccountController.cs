using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}")]
    public class ProviderAccountController : Controller
    {
        [Route("signout", Name = RouteNames.ProviderSignOut)]
        public IActionResult SignOut()
        {
            return SignOut(
                new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                {
                    RedirectUri = "",
                    AllowRefresh = true
                },
                CookieAuthenticationDefaults.AuthenticationScheme,
                WsFederationDefaults.AuthenticationScheme);
        }
    }
}
