using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses
{
    public class GetBulkUploadAddDraftApprenticeshipsResult
    {
        public IEnumerable<BulkUploadAddDraftApprenticeshipsResult> BulkUploadAddDraftApprenticeshipsResponse { get; set; }

        public GetBulkUploadAddDraftApprenticeshipsResult()
        {
            BulkUploadAddDraftApprenticeshipsResponse = new List<BulkUploadAddDraftApprenticeshipsResult>();
        }
    }
}
