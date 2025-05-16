using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Learners;

public class AddAnotherLearnerSelectedRequest : IAuthorizationContextModel
{
    public long ProviderId { get; set; }
    public Guid CacheKey { get; set; }
    public long LearnerDataId { get; set; }
    public string CohortReference { get; set; }
    public long CohortId { get; set; }
}
