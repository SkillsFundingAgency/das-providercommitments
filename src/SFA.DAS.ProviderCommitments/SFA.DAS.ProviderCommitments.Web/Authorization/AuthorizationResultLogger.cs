using SFA.DAS.ProviderCommitments.Authorization;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization.Handlers;

namespace SFA.DAS.ProviderCommitments.Web.Authorization;

public class AuthorizationResultLogger : IAuthorizationHandler
{
    public string Prefix => _authorizationHandler.Prefix;
        
    private readonly IAuthorizationHandler _authorizationHandler;
    private readonly ILogger<AuthorizationResultLogger> _logger;

    public AuthorizationResultLogger(IAuthorizationHandler authorizationHandler, ILogger<AuthorizationResultLogger> logger)
    {
        _authorizationHandler = authorizationHandler;
        _logger = logger;
    }

    public async Task<AuthorizationResult> GetAuthorizationResult(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext)
    {
        var authorizationResult = await _authorizationHandler.GetAuthorizationResult(options, authorizationContext).ConfigureAwait(false);           
            
        authorizationContext.TryGet("AccountId", out long? accountId);
        accountId = accountId.HasValue ? accountId : 0;
        authorizationContext.TryGet("HashedAccountId", out string hashedAccountId);
        authorizationContext.TryGet("UserRef", out Guid userRef);
        var message = $"Finished running handler with prefix '{Prefix}' for options '{string.Join(", ", options)}' and context  AccountId: '{accountId }' HashedAccountId: '{hashedAccountId}' UserRef: '{userRef}'  with result '{authorizationResult}'";

        if (authorizationResult.IsAuthorized)
        {
            _logger.LogInformation(message);
        }
        else
        {
            _logger.LogWarning(message);
        }

        return authorizationResult;
    }
}