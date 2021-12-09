using System;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class ReviewCohortSummaryViewModel
    {
        public string CohortReference { get; set; }
        public string EmployerName { get; set; }
        public int NumberOfApprentices { get; set; }
        public string LastMessage { get; set; }
        public DateTime DateReceived { get; set; }
    }
}
