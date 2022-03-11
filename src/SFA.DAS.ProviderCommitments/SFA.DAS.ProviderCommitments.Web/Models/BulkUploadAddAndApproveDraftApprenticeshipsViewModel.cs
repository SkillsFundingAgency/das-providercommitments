using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class BulkUploadAddAndApproveDraftApprenticeshipsViewModel
    {
        public List<BulkUploadDraftApprenticeshipViewModel> BulkUploadDraftApprenticeshipsViewModel { get; set; }
        public long ProviderId { get; set; }
    }
}
