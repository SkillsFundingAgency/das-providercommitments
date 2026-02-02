using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;

public class SelectMultipleLearnerRecordsFilterRequest : IAuthorizationContextModel
{
    public Guid? CacheKey { get; set; }
    public long ProviderId { get; set; }
    public string SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int? StartMonth { get; set; }
    public int StartYear { get; set; } = DateTime.UtcNow.Year;
    public bool ClearFilter { get; set; }
}