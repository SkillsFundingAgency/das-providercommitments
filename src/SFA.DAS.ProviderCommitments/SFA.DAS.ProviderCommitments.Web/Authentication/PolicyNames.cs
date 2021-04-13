namespace SFA.DAS.ProviderCommitments.Web.Authentication
{
    public static class PolicyNames
    {
        public static string ProviderPolicyName => "ProviderPolicy";

        public static string HasViewerOrAbovePermission => "HasViewerOrAbovePermission";

        public static string HasContributorOrAbovePermission => "HasContributorOrAbovePermission";

        public static string HasContributorWithApprovalOrAbovePermission => "HasContributorWithApprovalOrAbovePermission";

        public static string HasAccountOwnerPermission => "HasAccountOwnerPermission";
    }
}
