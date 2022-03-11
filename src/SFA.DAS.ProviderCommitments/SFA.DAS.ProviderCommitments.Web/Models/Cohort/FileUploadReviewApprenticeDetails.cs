namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
<<<<<<< HEAD:src/SFA.DAS.ProviderCommitments/SFA.DAS.ProviderCommitments.Web/Models/Cohort/ReviewApprenticeDetailsForFileUploadCohort.cs
    public class ReviewApprenticeDetailsForFileUploadCohort
=======
    public class FileUploadReviewApprenticeDetails
>>>>>>> master:src/SFA.DAS.ProviderCommitments/SFA.DAS.ProviderCommitments.Web/Models/Cohort/FileUploadReviewApprenticeDetails.cs
    {
        public string Name { get; set; }
        public string TrainingCourse { get; set; }
        public string ULN { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public string TrainingDates { get; set; }
        public decimal? Price { get; set; }
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
