using System.Security.Claims;
using SFA.DAS.ProviderCommitments.Web.Authentication;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class ServiceClaimExtensions
    {
        private static readonly List<string> ServiceClaimsList = Enum.GetNames(typeof(ServiceClaim)).ToList();

        public static bool IsServiceClaim(this string claim)
        {
            return ServiceClaimsList.Contains(claim);
        }

        public static bool HasPermission(this ClaimsPrincipal user, ServiceClaim minimumRequiredClaim)
        {
            var serviceClaims = user
                .FindAll(c => c.Type == ProviderClaims.Service)
                .Select(c => c.Value)
                .ToList();

            ServiceClaim? highestClaim = null;

            if (serviceClaims.Contains(ServiceClaim.DAA.ToString())) highestClaim = ServiceClaim.DAA;
            else if (serviceClaims.Contains(ServiceClaim.DAB.ToString())) highestClaim = ServiceClaim.DAB;
            else if (serviceClaims.Contains(ServiceClaim.DAC.ToString())) highestClaim = ServiceClaim.DAC;
            else if (serviceClaims.Contains(ServiceClaim.DAV.ToString())) highestClaim = ServiceClaim.DAV;

            return highestClaim.HasValue && highestClaim.Value >= minimumRequiredClaim;
        }
    }
}
