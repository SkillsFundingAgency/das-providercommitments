namespace SFA.DAS.ProviderCommitments.Features
{
    public class ProviderFeature
    {
        private const string Prefix = "ProviderFeature.";
        public const string ProviderCreateCohortV2 = Prefix + "ProviderCreateCohortV2";
        public const string ApprenticeDetailsV2 = Prefix + "ApprenticeDetailsV2";
        public const string ManageApprenticesV2 = Prefix + ManageApprenticesV2WithoutPrefix;
        public const string ManageApprenticesV2WithoutPrefix = "ManageApprenticesV2";
        public const string BulkUploadV2 = Prefix + BulkUploadV2WithoutPrefix;
        public const string BulkUploadV2WithoutPrefix = "BulkUploadV2";
        public const string DeliveryModel = Prefix + "DeliveryModel";
    }
}