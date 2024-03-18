using System.Security.Claims;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;

namespace SFA.DAS.ProviderCommitments.Web.Authentication;

public class AuthenticationService(IHttpContextAccessor httpContextAccessor, ILogger<AuthenticationService> logger) : IAuthenticationService, IAuthenticationServiceForApim
{
    public string UserId => GetUserClaimAsString(ProviderClaims.Upn) ?? GetUserClaimAsString("sub");
    public string UserName => GetUserClaimAsString(ProviderClaims.Name) ?? GetUserClaimAsString(ProviderClaims.DisplayName);
    public string UserEmail => GetUserClaimAsString(ProviderClaims.Email) ?? GetUserClaimAsString("email");

    public bool IsUserAuthenticated()
    {
        logger.LogWarning("AuthenticationService.IsUserAuthenticated(). HttpContextAccessor null?: {Check1}. HttpContext null?: {Check2}. IsAuthenticated: {IsAuthed}", 
            httpContextAccessor == null,
            httpContextAccessor.HttpContext == null,
            httpContextAccessor?.HttpContext?.User.Identity?.IsAuthenticated);
        
        return httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
    }

    public bool TryGetUserClaimValue(string key, out string value)
    {
        var claimsIdentity = (ClaimsIdentity)httpContextAccessor.HttpContext.User.Identity;
        var claim = claimsIdentity.FindFirst(key);
        var exists = claim != null;

        value = exists ? claim.Value : null;

        return exists;
    }
        
    public bool TryGetUserClaimValues(string key, out IEnumerable<string> values)
    {
        var claimsIdentity = (ClaimsIdentity)httpContextAccessor.HttpContext.User.Identity;
        var claims = claimsIdentity.FindAll(key);

        values = claims.Select(c => c.Value).ToList();

        return values.Any();
    }

    public UserInfo UserInfo
    {
        get
        {
            if (IsUserAuthenticated())
            {
                return new UserInfo
                {
                    UserId = UserId,
                    UserDisplayName = UserName,
                    UserEmail = UserEmail
                };
            }

            return null;
        }
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