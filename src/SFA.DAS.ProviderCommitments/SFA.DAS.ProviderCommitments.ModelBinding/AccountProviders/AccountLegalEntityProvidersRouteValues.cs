using System.ComponentModel.DataAnnotations;
using SFA.DAS.ProviderCommitments.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.RouteValues.AccountProviders
{
    public class AccountLegalEntityProvidersRouteValues : IAuthorizationContextModel
    {
        [Required]
        public long? AccountLegalEntityId { get; set; }
    }
}
