using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;

public class SelectMultipleLearnerRecordsAddRequest : IAuthorizationContextModel
{
    public Guid? CacheKey { get; set; }
    public long ProviderId { get; set; }
    public LearnerSummary Learner { get; set; }
}