namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;
public class ChangeEmployerRedirectRequest
{
    public Guid? CacheKey { get; set; }
    public long ProviderId { get; set; }
    public string EmployerAccountLegalEntityPublicHashedId { get; set; }
    public string EmployerAccountName { get; set; }
}