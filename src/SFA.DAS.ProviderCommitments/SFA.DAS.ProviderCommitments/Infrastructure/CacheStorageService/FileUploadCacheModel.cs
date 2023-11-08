using System.Collections.Generic;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Infrastructure.CacheStorageService
{
    public class FileUploadCacheModel
    {
        public List<CsvRecord> CsvRecords { get; set; }
        public long? FileUploadLogId { get; set; }
    }
}