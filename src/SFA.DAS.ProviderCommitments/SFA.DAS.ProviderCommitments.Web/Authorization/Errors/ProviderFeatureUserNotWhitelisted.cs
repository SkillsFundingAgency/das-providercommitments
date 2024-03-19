using SFA.DAS.ProviderCommitments.Authorization;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.Errors;

public class ProviderFeatureUserNotWhitelisted : AuthorizationError
{
    public ProviderFeatureUserNotWhitelisted() : base("Provider feature user not whitelisted")
    {
    }
}