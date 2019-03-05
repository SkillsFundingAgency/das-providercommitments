using System.ComponentModel.DataAnnotations;
using SFA.DAS.Authorization;

namespace SFA.DAS.ProviderCommitments.Web.RouteValues.AccountProviders
{
    public class AccountProvidersRouteValues : IAuthorizationContextModel
    {
        [Required]
        public long? AccountId { get; set; }
    }
}
