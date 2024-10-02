using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate
{
    public class FileUploadValidateDataRequest : IRequest<FileUploadValidateDataResponse>
    {
        public long ProviderId { get; set; }
        // TODO Remove
        public bool RplDataExtended => true;
        public List<CsvRecord> CsvRecords { get; set; }
        public IFormFile Attachment { get; set; }
    }
}
