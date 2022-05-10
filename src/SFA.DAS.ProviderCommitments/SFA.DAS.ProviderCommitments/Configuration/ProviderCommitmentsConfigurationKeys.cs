namespace SFA.DAS.ProviderCommitments.Configuration
{
    public static class ProviderCommitmentsConfigurationKeys
    {
        public const string Encoding = "SFA.DAS.Encoding";
        public const string ProviderCommitments = "SFA.DAS.ProviderCommitments";
        public const string AuthenticationSettings = "AuthenticationSettings";
        public const string CommitmentsClientApiConfiguration = "CommitmentsClientApi";
        public const string ProviderRelationshipsApiConfiguration = "ProviderRelationshipsApi";
        public const string ApprovalsOuterApiConfiguration = "ApprovalsOuterApi";
        public const string FeaturesConfiguration = "Features";
        public const string ZenDeskConfiguration = "ZenDesk";
        public const string DataProtectionConnectionStrings = "DataProtection";
        public const string BlobStorageSetttings = "BlobStorage";
        public const string ProviderSharedUIConfiguration = "ProviderSharedUIConfiguration";
        public const string BulkUploadFileValidationConfiguration = "BulkUploadFileValidationConfiguration";
        public static string PasAccountApiClient => PAS.Account.Api.ClientV2.Configuration.ConfigurationKeys.PasAccountApiClient;
    }
}