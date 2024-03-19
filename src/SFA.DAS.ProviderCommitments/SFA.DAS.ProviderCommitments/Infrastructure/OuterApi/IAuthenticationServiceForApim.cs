namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;

public interface IAuthenticationServiceForApim
{
    bool IsUserAuthenticated();
    string UserName { get; }
    string UserId { get; }
    string UserEmail { get; }

}