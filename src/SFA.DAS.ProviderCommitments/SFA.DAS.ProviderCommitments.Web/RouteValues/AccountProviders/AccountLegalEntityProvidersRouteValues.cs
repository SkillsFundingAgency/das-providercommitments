using System.ComponentModel.DataAnnotations;
using SFA.DAS.Authorization;

namespace SFA.DAS.ProviderCommitments.Web.RouteValues.AccountProviders
{
    public class AccountLegalEntityProvidersRouteValues : IAuthorizationContextModel
    {
        [Required]
        public long? AccountLegalEntityId { get; set; }
    }
}
