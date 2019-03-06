using System.ComponentModel.DataAnnotations;
using SFA.DAS.ProviderCommitments.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.RouteValues.AccountProviders
{
    public class AccountProvidersRouteValues : IAuthorizationContextModel
    {
        [Required]
        public long? AccountId { get; set; }
    }
}
