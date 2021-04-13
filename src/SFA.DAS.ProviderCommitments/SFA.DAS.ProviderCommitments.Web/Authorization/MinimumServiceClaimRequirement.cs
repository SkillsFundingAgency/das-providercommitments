using Microsoft.AspNetCore.Authorization;
using SFA.DAS.ProviderCommitments.Web.Authentication;

namespace SFA.DAS.ProviderCommitments.Web.Authorization
{
    public class MinimumServiceClaimRequirement : IAuthorizationRequirement
    {
        public ServiceClaim MinimumServiceClaim { get; set; }

        public MinimumServiceClaimRequirement(ServiceClaim minimumServiceClaim)
        {
            MinimumServiceClaim = minimumServiceClaim;
        }
    }
}
