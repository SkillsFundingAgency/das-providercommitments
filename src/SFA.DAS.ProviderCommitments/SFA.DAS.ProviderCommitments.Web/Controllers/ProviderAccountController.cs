using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using SFA.DAS.ProviderCommitments.Web.Authentication;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    public class ProviderAccountController(ProviderSharedUIConfiguration providerSharedUiConfiguration)
        : Controller
    {
        [Route("", Name = RouteNames.ProviderAccountIndex)]
        public IActionResult Index([FromServices] IHttpContextAccessor httpContextAccessor)
        {
            var ukprn = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ProviderClaims.Ukprn)?.Value;
            return RedirectToRoute(RouteNames.Cohort, new { providerId = ukprn });
        }
        
        [Route("{providerId}/dashboard", Name = RouteNames.Dashboard)]
        public IActionResult Dashboard()
        {
            return RedirectPermanent(providerSharedUiConfiguration.DashboardUrl);
        }
        
        [Route("signout", Name = RouteNames.ProviderSignOut)]
        [Route("service/signout")]
        public IActionResult SignOut([FromQuery] bool autoSignOut = false)
        {
            TempData["AutoSignOut"] = autoSignOut;
            return SignOut(
                new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                {
                    AllowRefresh = true
                },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }
        
        [Route("~/signed-out", Name = "signed-out")]
        [AllowAnonymous]
        public IActionResult ProviderSignedOut()
        {
            var autoSignOut = TempData["AutoSignOut"] as bool? ?? false;
            return autoSignOut ? View("AutoSignOut") : RedirectToRoute(RouteNames.ProviderAccountIndex);
        }
    }
}