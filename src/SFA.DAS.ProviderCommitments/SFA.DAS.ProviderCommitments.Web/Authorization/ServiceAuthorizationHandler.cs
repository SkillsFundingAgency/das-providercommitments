using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.Handlers;
using SFA.DAS.Authorization.Results;

namespace SFA.DAS.ProviderCommitments.Web.Authorization
{
    public class ServiceAuthorizationHandler : IAuthorizationHandler
    {
        public string Prefix => "Service.";

        public Task<AuthorizationResult> GetAuthorizationResult(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext)
        {
            var authorizationResult = new AuthorizationResult();
            var service = authorizationContext.Get<string>(AuthorizationContextKeys.Service);
            
            if (service != ServiceValues.DAA)
            {
                authorizationResult.AddError(new ServiceNotAuthorized());
            }

            return Task.FromResult(authorizationResult);
        }
    }
}