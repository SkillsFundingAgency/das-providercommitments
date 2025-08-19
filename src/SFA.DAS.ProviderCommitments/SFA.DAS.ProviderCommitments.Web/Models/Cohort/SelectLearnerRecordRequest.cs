using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;

public class SelectLearnerRecordRequest : IAuthorizationContextModel
{
    public long ProviderId { get; set; }
    public string EmployerAccountLegalEntityPublicHashedId { get; set; }
    public Guid? CacheKey { get; set; }
    public Guid? ReservationId { get; set; }
    public long? AccountLegalEntityId { get; set; }
    public string SearchTerm { get; set; }
    public string SortField { get; set; }
    public bool ReverseSort { get; set; }
    public int Page { get; set; } = 1;
    public string CohortReference { get; set; }
    public long? CohortId { get; set; }
    public int? StartMonth { get; set; }
    public int StartYear { get; set; } = 2025;
}

