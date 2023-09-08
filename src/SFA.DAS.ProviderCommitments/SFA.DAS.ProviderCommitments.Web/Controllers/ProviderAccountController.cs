using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    public class ProviderAccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ProviderSharedUIConfiguration _providerSharedUiConfiguration;

        public ProviderAccountController(IConfiguration configuration, ProviderSharedUIConfiguration providerSharedUiConfiguration)
        {
            _configuration = configuration;
            _providerSharedUiConfiguration = providerSharedUiConfiguration;
        }
        
        [AllowAnonymous]
        [Route("signout", Name = RouteNames.ProviderSignOut)]
        public IActionResult SignOut()
        {
            var authScheme = _configuration.GetSection(ProviderCommitmentsConfigurationKeys.UseDfeSignIn).Get<bool>() 
                ? OpenIdConnectDefaults.AuthenticationScheme
                : WsFederationDefaults.AuthenticationScheme;

            return SignOut(
                new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                {
                    AllowRefresh = true
                },
                CookieAuthenticationDefaults.AuthenticationScheme,
                authScheme);
        }

        [HttpGet]
        [Route("{providerId}/dashboard")]
        public IActionResult Dashboard()
        {
            return RedirectPermanent(_providerSharedUiConfiguration.DashboardUrl);
        }
    }
}
