namespace SFA.DAS.ProviderCommitments.Web.Authentication;

public static class PolicyNames
{
    public static string ProviderPolicyName => "ProviderPolicy";
    public static string HasViewerOrAbovePermission => nameof(HasViewerOrAbovePermission);
    public static string HasContributorOrAbovePermission => nameof(HasContributorOrAbovePermission);
    public static string HasContributorWithApprovalOrAbovePermission => nameof(HasContributorWithApprovalOrAbovePermission);
    public static string HasAccountOwnerPermission => nameof(HasAccountOwnerPermission);
    public static string AccessApprenticeship => nameof(AccessApprenticeship);
    public static string AccessCohort => nameof(AccessCohort);
    public static string CreateCohort => nameof(CreateCohort);
       
}