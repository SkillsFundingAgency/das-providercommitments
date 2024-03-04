namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class CourseDataLockViewModel
    {
        public string CurrentTrainingName { get; set; }        

        public DateTime CurrentStartDate { get; set; }

        public DateTime? CurrentEndDate { get; set; }

        public string IlrTrainingName { get; set; }

        public DateTime? IlrEffectiveFromDate { get; set; }

        public DateTime? IlrEffectiveToDate { get; set; }
    }
}
