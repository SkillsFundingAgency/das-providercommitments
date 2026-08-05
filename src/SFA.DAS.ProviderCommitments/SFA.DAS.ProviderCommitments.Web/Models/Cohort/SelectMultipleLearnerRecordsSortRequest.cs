using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;

public class SelectMultipleLearnerRecordsSortRequest : IAuthorizationContextModel
{
    public Guid? CacheKey { get; set; }
    public long ProviderId { get; set; }
    public string SortField { get; set; }
}