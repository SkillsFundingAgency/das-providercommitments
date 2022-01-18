namespace SFA.DAS.ProviderCommitments.Configuration
{
    public class RedisConnectionSettings
    {
        public string RedisConnectionString { get; set; }
        public string BulkUploadCacheDatabase { get; set; }
        public string DataProtectionKeysDatabase { get; set; }
    }
}
