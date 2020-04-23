using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class ReviewCohortSummaryViewModel2
    {
        public string CohortReference { get; set; }
        public string EmployerName { get; set; }
        public int NumberOfApprentices { get; set; }
        public string LastMessage { get; set; }
    }
}
