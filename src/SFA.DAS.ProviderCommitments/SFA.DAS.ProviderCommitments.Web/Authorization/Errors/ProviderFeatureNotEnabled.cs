using SFA.DAS.ProviderCommitments.Authorization;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.Errors;

public class ProviderFeatureNotEnabled : AuthorizationError
{
    public ProviderFeatureNotEnabled() : base("Provider feature is not enabled")
    {
    }
}