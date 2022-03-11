namespace SFA.DAS.ProviderCommitments.Configuration
{
    public static class ProviderCommitmentsConfigurationKeys
    {
        public const string Encoding = "SFA.DAS.Encoding";
        public const string ProviderCommitments = "SFA.DAS.ProviderCommitments";
        public static string AuthenticationSettings = "AuthenticationSettings";
        public static string CommitmentsClientApiConfiguration = "CommitmentsClientApi";
        public static string ApprovalsOuterApiConfiguration = "ApprovalsOuterApi";
        public static string FeaturesConfiguration = "Features";
        public static string ZenDeskConfiguration = "ZenDesk";
        public static string DataProtectionConnectionStrings = "DataProtection";
        public static string BlobStorageSetttings = "BlobStorage";
        public static string ProviderSharedUIConfiguration = "ProviderSharedUIConfiguration";
        public static string BulkUploadFileValidationConfiguration = "BulkUploadFileValidationConfiguration";
        public static string PasAccountApiClient => PAS.Account.Api.ClientV2.Configuration.ConfigurationKeys.PasAccountApiClient;
    }
}