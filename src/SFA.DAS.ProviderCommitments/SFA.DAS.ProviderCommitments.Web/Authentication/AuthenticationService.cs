using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.ProviderCommitments.Web.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        public bool IsUserAuthenticated()
        {
            return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public bool TryGetUserClaimValues(string key, out IList<string> value)
        {
            var claimsIdentity = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            var claims = claimsIdentity.Claims.Where(x => x.Type == key).Select(y => y.Value).ToList();
            var exists = claims.Any();

            value = exists ? claims : null;

            return exists;
        }

        public bool TryGetFirstUserClaimValue(string key, out string value)
        {
            if (TryGetUserClaimValues(key, out var result))
            {
                if (result.Any())
                {
                    value = result.First();
                    return true;
                }
            }

            value = string.Empty;
            return false;
        }
    }
}