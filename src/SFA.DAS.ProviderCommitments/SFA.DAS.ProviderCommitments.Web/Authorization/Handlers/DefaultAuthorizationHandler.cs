using SFA.DAS.ProviderCommitments.Authorization;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.Handlers
{
    public interface IDefaultAuthorizationHandler
    {        
        Task<AuthorizationResult> GetAuthorizationResult(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext);
    }
    
    /// <summary>
    /// Actual implementation been done in the calling project to give the flexibility of the usage of this handler.
    /// </summary>
    public class DefaultAuthorizationHandler : IDefaultAuthorizationHandler
    {
        public Task<AuthorizationResult> GetAuthorizationResult(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext)
        {
            return Task.FromResult(new AuthorizationResult());
        }      
    }
}
