using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.Authorization;

public class DefaultAuthorizationContextProvider : IAuthorizationContextProvider
{
    public IAuthorizationContext GetAuthorizationContext()
    {
        return new AuthorizationContext();
    }
}