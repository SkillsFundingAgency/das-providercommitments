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

        public string UserId => GetUserClaimAsString(ProviderClaims.Upn);
        public string UserName => GetUserClaimAsString(ProviderClaims.Name);
        public string UserEmail => GetUserClaimAsString(ProviderClaims.Email);

        public bool IsUserAuthenticated()
        {
            return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public bool TryGetUserClaimValue(string key, out string value)
        {
            var claimsIdentity = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            var claim = claimsIdentity.FindFirst(key);
            var exists = claim != null;

            value = exists ? claim.Value : null;

            return exists;
        }
        
        public bool TryGetUserClaimValues(string key, out IEnumerable<string> values)
        {
            var claimsIdentity = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            var claims = claimsIdentity.FindAll(key);

            values = claims.Select(c => c.Value).ToList();

            return values.Any();
        }

        private string GetUserClaimAsString(string claim)
        {
            if (IsUserAuthenticated() && TryGetUserClaimValue(claim, out var value))
            {
                return value;
            }
            return null;
        }
    }
}