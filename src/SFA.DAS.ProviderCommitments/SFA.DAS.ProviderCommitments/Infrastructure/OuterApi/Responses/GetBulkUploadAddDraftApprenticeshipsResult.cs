using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses
{
    public class GetBulkUploadAddDraftApprenticeshipsResult
    {
        public GetBulkUploadAddDraftApprenticeshipsResult()
        {
            BulkUploadAddDraftApprenticeshipsResponse = new List<BulkUploadAddDraftApprenticeshipsResult>();
        }

        public IEnumerable<BulkUploadAddDraftApprenticeshipsResult> BulkUploadAddDraftApprenticeshipsResponse { get; set; }
    }
}
