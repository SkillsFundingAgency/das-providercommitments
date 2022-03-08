namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class FileUploadReviewApprenticeDetails
    {
        public string Name { get; set; }
        public string TrainingCourse { get; set; }
        public string ULN { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public string TrainingDates { get; set; }
        public int Price { get; set; }
        public int? FundingBandCap { get; set; }

        public bool ExceedsFundingBandCap
        {
            get
            {
                if (FundingBandCap.HasValue)
                {
                    return Price > FundingBandCap.Value;
                }

                return false;
            }
        }     
    }
}
