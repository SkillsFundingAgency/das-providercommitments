using System;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class WithEmployerSummaryViewModel
    {
        public string CohortReference { get; set; }
        public string EmployerName { get; set; }
        public int NumberOfApprentices { get; set; }
        public string LastMessage { get; set; }
        public DateTime DateSentToEmployer { get; set; }
    }
}
