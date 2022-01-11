namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class CohortsByProviderRequest
    {
        public long ProviderId { get; set; }
        public string SortField { get; set; }
        public bool ReverseSort { get; set; }
    }
}
