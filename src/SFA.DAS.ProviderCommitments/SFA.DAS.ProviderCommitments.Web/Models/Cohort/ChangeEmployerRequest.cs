namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;
public class ChangeEmployerRequest
{
    public Guid? CacheKey { get; set; }
    public long ProviderId { get; set; }
    public string SortField { get; set; }
    public bool ReverseSort { get; set; }
    public string SearchTerm { get; set; }
    public bool UseLearnerData { get; set; }
}
