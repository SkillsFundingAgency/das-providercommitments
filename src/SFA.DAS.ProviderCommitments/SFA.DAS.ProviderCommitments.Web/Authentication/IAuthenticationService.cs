using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Authentication
{
    public interface IAuthenticationService
    {
        bool IsUserAuthenticated();
        bool TryGetUserClaimValue(string key, out string value);
        bool TryGetUserClaimValues(string key, out IEnumerable<string> values);
        string UserName { get; }
        string UserId { get; }
        string UserEmail { get; }
        UserInfo UserInfo { get; }
    }
}