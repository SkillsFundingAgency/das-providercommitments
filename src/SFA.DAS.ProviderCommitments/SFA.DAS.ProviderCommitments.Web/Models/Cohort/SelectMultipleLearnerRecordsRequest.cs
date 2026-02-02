using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;

public class SelectMultipleLearnerRecordsRequest : IAuthorizationContextModel
{
    public Guid? CacheKey { get; set; }
    public long ProviderId { get; set; }
}