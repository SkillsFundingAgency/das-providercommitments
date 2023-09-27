using MediatR;
using Microsoft.AspNetCore.Http;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate
{
    public class FileUploadValidateDataRequest : IRequest<FileUploadValidateDataResponse>
    {
        public long ProviderId { get; set; }
        public bool RplDataExtended { get; set; }
        public List<CsvRecord> CsvRecords { get; set; }
        public IFormFile Attachment { get; set; }
        public CommitmentsV2.Types.UserInfo UserInfo { get; set; }
    }
}
