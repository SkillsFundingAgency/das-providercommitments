using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Authentication
{
    public interface IAuthenticationService
    {
        bool IsUserAuthenticated();
        bool TryGetUserClaimValues(string key, out IList<string> value);
        bool TryGetFirstUserClaimValue(string key, out string value);
    }
}