using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class BulkUploadValidateApiRequest
    {
        public long ProviderId { get; set; }
        public IEnumerable<BulkUploadAddDraftApprenticeshipRequest> CsvRecords { get; set; }
        public UserInfo UserInfo { get; set; }
    }
}
