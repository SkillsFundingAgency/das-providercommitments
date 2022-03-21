namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class ReviewApprenticeDetailsForExistingCohort
    {
        public string Name { get; set; }
        public string TrainingCourse { get; set; }
        public string ULN { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public string TrainingDates { get; set; }
        public decimal? Price { get; set; }
        public int? FundingBandCapForExistingCohort { get; set; }

        public bool ExceedsFundingBandCapForExistingCohort
        {
            get
            {
                if (FundingBandCapForExistingCohort.HasValue)
                {
                    return Price > FundingBandCapForExistingCohort.Value;
                }

                return false;
            }
        }     
    }
}
