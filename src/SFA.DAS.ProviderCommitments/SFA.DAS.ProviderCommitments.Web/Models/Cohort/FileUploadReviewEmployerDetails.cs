using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class FileUploadReviewEmployerDetails
    {
        public string LegalEntityName { get; set; }
        public string AgreementId { get; set; }
        public string CohortRef { get; set; }
        public List<FileUploadReviewCohortDetail> CohortDetails { get; set; }
    }
}

