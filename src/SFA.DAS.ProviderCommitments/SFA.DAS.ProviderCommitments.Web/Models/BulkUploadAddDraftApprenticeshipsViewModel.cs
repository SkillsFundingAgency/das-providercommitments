using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class BulkUploadAddDraftApprenticeshipsViewModel
    {
        public List<BulkUploadDraftApprenticeshipViewModel> BulkUploadDraftApprenticeshipsViewModel { get; set; }
        public long ProviderId { get; set; }
    }

    public class BulkUploadDraftApprenticeshipViewModel
    {   
        public string CohortReference { get; set; }
        public int NumberOfApprenticeships { get; set; }
        public string EmployerName { get; set; }
    }
}
