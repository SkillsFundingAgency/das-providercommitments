namespace SFA.DAS.ProviderCommitments.Authorization;

public abstract class AuthorizationError
{
    public string Message { get; }

    protected AuthorizationError(string message)
    {
        Message = message;
    }
}