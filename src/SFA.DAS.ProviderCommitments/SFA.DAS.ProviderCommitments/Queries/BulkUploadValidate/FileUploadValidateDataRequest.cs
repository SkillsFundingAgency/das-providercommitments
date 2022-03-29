using MediatR;
using Microsoft.AspNetCore.Http;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate
{
    public class FileUploadValidateDataRequest : IRequest
    {
        public long ProviderId { get; set; }
        public List<CsvRecord> CsvRecords { get; set; }
        public IFormFile Attachment { get; set; }
    }
}
