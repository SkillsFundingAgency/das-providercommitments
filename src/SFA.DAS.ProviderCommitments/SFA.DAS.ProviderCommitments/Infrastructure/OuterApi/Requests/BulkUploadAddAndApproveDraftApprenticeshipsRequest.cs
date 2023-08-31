using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class BulkUploadAddAndApproveDraftApprenticeshipsRequest : ApimSaveDataRequest
    {
        public BulkUploadAddAndApproveDraftApprenticeshipsRequest() 
        { 
            BulkUploadAddAndApproveDraftApprenticeships = new List<BulkUploadAddDraftApprenticeshipRequest>(); 
        }

        public long ProviderId { get; set; }
        public long? FileUploadLogId { get; set; }
        public IEnumerable<BulkUploadAddDraftApprenticeshipRequest> BulkUploadAddAndApproveDraftApprenticeships { get; set; }
    }
}
