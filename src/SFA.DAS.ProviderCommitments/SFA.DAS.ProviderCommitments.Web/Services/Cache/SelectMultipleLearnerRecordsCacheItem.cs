using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.Services.Cache;
public class SelectMultipleLearnerRecordsCacheItem : ICacheModel
{
    public Guid Key { get; }
    public Guid CacheKey => Key;

    public SelectMultipleLearnerRecordsCacheItem(Guid key)
    {
        Key = key;
    }

    public long ProviderId { get; set; }
    public string EmployerAccountLegalEntityPublicHashedId { get; set; }
    public bool UseLearnerData { get; set; }
    public long? CohortId { get; set; }
    public long? AccountLegalEntityId { get; set; }
    public string CohortReference { get; set; }
    public string EmployerAccountName { get; set; }
    public string SearchTerm { get; set; }
    public string SortField { get; set; }
    public bool ReverseSort { get; set; }
    public string StartMonth { get; set; }
    public string StartYear { get; set; } = DateTime.UtcNow.Year.ToString();
}