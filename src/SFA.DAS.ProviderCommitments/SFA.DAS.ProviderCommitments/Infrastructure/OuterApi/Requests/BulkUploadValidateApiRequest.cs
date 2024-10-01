using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class BulkUploadValidateApimRequest : ApimSaveDataRequest
    {
        public long ProviderId { get; set; }

        // TODO: Needs to be removed
        public bool RplDataExtended = true;
        public IEnumerable<BulkUploadAddDraftApprenticeshipRequest> CsvRecords { get; set; }
        public long FileUploadLogId { get; set; }
    }
}
