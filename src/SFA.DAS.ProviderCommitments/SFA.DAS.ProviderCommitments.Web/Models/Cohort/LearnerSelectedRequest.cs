using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;

public class LearnerSelectedRequest : IAuthorizationContextModel
{
    public long ProviderId { get; set; }
    public Guid CacheKey { get; set; }
    public long LearnerDataId { get; set; }
    public string EmployerAccountLegalEntityPublicHashedId { get; set; }
    public long AccountLegalEntityId { get; set; }
}
