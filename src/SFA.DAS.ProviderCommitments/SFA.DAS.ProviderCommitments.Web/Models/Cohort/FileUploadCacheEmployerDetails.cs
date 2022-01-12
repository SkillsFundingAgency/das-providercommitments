using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class FileUploadCacheEmployerDetails
    {
        public string EmployerName { get; set; }
        public string AgreementId { get; set; }
        public string CohortRef { get; set; }
        public List<FileUploadCacheCohortDetail> CohortDetails { get; set; }
    }
}

