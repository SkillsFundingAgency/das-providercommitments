namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class FileUploadReviewCohortDetail
    {
        public const string EmptyCohortRefText = "This will be created when you save or send to employers";
        public string CohortRef { get; set; }
        public int NumberOfApprentices { get; set; }
        public int TotalCost { get; set; }

        public string CohortRefText => CohortRef ?? EmptyCohortRefText;

        public string NumberOfApprenticeshipsText
        {
            get
            {
                var text = NumberOfApprentices + " " + "apprentice";
                if (NumberOfApprentices > 1)
                {
                    text += "s";
                }

                return text;
            }
        }

        public string ReviewApprenticeshipsText
        {
            get
            {
                return (NumberOfApprentices > 1) ? "Review apprentices" : "Review apprentice";
            }          
        }
    }
}

