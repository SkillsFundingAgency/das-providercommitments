using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public interface IBulkUploadFileParser
    {
        List<CsvRecord> GetCsvRecords(long providerId, IFormFile attachment);
    }
}
