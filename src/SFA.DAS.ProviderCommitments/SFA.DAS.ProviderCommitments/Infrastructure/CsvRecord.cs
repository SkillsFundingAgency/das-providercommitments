namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class CsvRecord
    {
        public string CohortRef { get; set; }

        public string ULN { get; set; }

        public string FamilyName { get; set; }

        public string GivenNames { get; set; }

        public string DateOfBirth { get; set; }

        public string StdCode { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string TotalPrice { get; set; }

        public string EPAOrgID { get; set; }

        public string ProviderRef { get; set; }

        public string AgreementId { get; set; }
        
        public string EmailAddress { get; set; }

        public string RecognisePriorLearning { get; set; }
        
        public string DurationReducedBy { get; set; }

        public string PriceReducedBy { get; set; }

        public string TrainingTotalHours { get; set; }

        public string IsDurationReducedByRPL { get; set; }

        public string TrainingHoursReduction { get; set; }
    }
}