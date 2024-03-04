using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization.Context;

namespace SFA.DAS.ProviderCommitments.Web.Authorization;

public class DefaultAuthorizationContextProvider : IAuthorizationContextProvider
{
    public IAuthorizationContext GetAuthorizationContext()
    {
        return new AuthorizationContext();
    }
}