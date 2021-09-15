using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class ChooseCohortSummaryViewModel
    {
        public string EmployerName { get; set; }
        public string CohortReference { get; set; }
        public string Status { get; set; }
        public int NumberOfApprentices { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
