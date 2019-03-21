using System.Security.Claims;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string Upn(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal?.Identity?.Upn();
        }
    }
}