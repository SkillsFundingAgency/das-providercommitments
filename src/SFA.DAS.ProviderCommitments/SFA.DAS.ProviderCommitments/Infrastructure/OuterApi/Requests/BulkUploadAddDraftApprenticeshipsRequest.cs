using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class BulkUploadAddDraftApprenticeshipsRequest : ApimSaveDataRequest 
    {
        public BulkUploadAddDraftApprenticeshipsRequest()
        {
            BulkUploadDraftApprenticeships = new List<BulkUploadAddDraftApprenticeshipRequest>();
        }

        public long ProviderId { get; set; }
        public long? FileUploadLogId { get; set; }
        public IEnumerable<BulkUploadAddDraftApprenticeshipRequest> BulkUploadDraftApprenticeships { get; set; }
    }
}
