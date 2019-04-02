namespace SFA.DAS.ProviderCommitments.Configuration
{
    public static class ProviderCommitmentsConfigurationKeys
    {
        public const string ProviderCommitments = "SFA.DAS.ProviderCommitments";
        public static string AuthenticationSettings = $"{ProviderCommitments}:AuthenticationSettings";
        public static string ApprenticeshipInfoServiceConfiguration = $"{ProviderCommitments}:ApprenticeshipInfoServiceConfiguration";
        public static string PublicAccountIdHashingConfiguration = $"{ProviderCommitments}:PublicAccountIdHashingConfiguration";
        public static string PublicAccountLegalEntityIdHashingConfiguration = $"{ProviderCommitments}:PublicAccountLegalEntityIdHashingConfiguration";
        public static string CommitmentsClientApiConfiguration = $"{ProviderCommitments}:CommitmentsClientApi";
        public static string FeatureConfiguration = $"{ProviderCommitments}:Features";
    }
}
