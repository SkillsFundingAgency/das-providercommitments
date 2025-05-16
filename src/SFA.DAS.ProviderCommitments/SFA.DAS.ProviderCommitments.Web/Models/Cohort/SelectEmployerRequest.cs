namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class SelectEmployerRequest
    {
        public long ProviderId { get; set; }
        public string SortField { get; set; }
        public bool ReverseSort { get; set; }
        public string SearchTerm { get; set; }
        public bool UseLearnerData { get; set; }
    }
}
