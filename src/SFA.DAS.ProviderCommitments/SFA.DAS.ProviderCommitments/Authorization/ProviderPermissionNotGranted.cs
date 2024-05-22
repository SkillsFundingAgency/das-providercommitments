namespace SFA.DAS.ProviderCommitments.Authorization;

public class ProviderPermissionNotGranted : AuthorizationError
{
    public ProviderPermissionNotGranted() : base("Provider permission is not granted")
    {
    }
}