using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization;

namespace SFA.DAS.ProviderCommitments.Web.Caching;

public class AuthorizationContextCache : IAuthorizationContextProvider
{
    private readonly Lazy<IAuthorizationContext> _authorizationContext;

    public AuthorizationContextCache(IAuthorizationContextProvider authorizationContextProvider)
    {
        _authorizationContext = new Lazy<IAuthorizationContext>(authorizationContextProvider.GetAuthorizationContext);
    }

    public IAuthorizationContext GetAuthorizationContext()
    {
        return _authorizationContext.Value;
    }
}