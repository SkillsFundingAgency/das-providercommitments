using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using SFA.DAS.ProviderCommitments.Web.Authentication;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class IdentityExtensions
    {
        public static string Upn(this IIdentity identity)
        {
            switch (identity)
            {
                case ClaimsIdentity claimsIdentity:
                    return claimsIdentity.Claims.FirstOrDefault(claim => claim.Type == ProviderClaims.Upn)?.Value;

                default:
                    return null;
            }
        }
    }
}
