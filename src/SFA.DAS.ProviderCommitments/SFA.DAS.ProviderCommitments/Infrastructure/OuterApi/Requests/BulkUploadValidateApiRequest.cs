using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class BulkUploadValidateApimRequest : ApimSaveDataRequest
    {
        public long ProviderId { get; set; }
        public bool RplDataExtended { get; set; }
        public IEnumerable<BulkUploadAddDraftApprenticeshipRequest> CsvRecords { get; set; }
        public long FileUploadLogId { get; set; }
    }
}
