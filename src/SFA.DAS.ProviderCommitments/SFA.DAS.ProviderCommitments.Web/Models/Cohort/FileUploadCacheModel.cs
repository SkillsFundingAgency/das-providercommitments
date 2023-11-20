using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class FileUploadCacheModel
    {
        public List<CsvRecord> CsvRecords { get; set; }
        public long? FileUploadLogId { get; set; }
    }
}