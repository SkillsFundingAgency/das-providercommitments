using SFA.DAS.Authorization.Errors;

namespace SFA.DAS.ProviderCommitments.Web.Authorization;

public class ProviderPermissionNotGranted : AuthorizationError
{
    public ProviderPermissionNotGranted() : base("Provider permission is not granted")
    {
    }
}