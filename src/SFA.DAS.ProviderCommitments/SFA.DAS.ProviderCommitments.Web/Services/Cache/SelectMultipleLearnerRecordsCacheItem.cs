using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Services.Cache;
public class SelectMultipleLearnerRecordsCacheItem : ICacheModel
{
    public Guid Key { get; }
    public Guid CacheKey => Key;

    public SelectMultipleLearnerRecordsCacheItem(Guid key)
    {
        Key = key;
        Filter = new LearnerRecordsFilterModel();
    }

    public long ProviderId { get; set; }
    public string EmployerAccountLegalEntityPublicHashedId { get; set; }
    public bool UseLearnerData { get; set; }
    public LearnerRecordsFilterModel Filter { get; set; }
    public long? CohortId { get; set; }
    public long? AccountLegalEntityId { get; set; }
    public Guid? ReservationId { get; set; }
    public string CohortReference { get; set; }
    public string EmployerAccountName { get; set; }
}