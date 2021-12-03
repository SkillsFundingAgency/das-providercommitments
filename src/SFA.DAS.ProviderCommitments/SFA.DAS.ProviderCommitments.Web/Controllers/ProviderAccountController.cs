using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}")]
    public class ProviderAccountController : Controller
    {
        private readonly ILinkGenerator _linkGenerator;

        public ProviderAccountController(ILinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        [Route("signout", Name = RouteNames.ProviderSignOut)]
        public IActionResult SignOut()
        {
            return SignOut(
                new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                {
                    RedirectUri = _linkGenerator.ProviderApprenticeshipServiceLink(""),
                    AllowRefresh = true
                },
                CookieAuthenticationDefaults.AuthenticationScheme,
                WsFederationDefaults.AuthenticationScheme);
        }
    }
}
