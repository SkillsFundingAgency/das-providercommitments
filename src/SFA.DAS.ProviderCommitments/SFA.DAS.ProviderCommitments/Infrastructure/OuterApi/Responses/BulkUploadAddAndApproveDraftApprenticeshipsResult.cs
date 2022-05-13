using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses
{
    public class BulkUploadAddAndApproveDraftApprenticeshipsResult
    {
        public IEnumerable<BulkUploadAddDraftApprenticeshipsResult> BulkUploadAddAndApproveDraftApprenticeshipResponse { get; set; } = new List<BulkUploadAddDraftApprenticeshipsResult>();
    }

    public class BulkUploadAddDraftApprenticeshipsResult
    {
        public BulkUploadAddDraftApprenticeshipsResult() { }

        public string CohortReference { get; set; }
        public int NumberOfApprenticeships { get; set; }
        public string EmployerName { get; set; }
    }
}
