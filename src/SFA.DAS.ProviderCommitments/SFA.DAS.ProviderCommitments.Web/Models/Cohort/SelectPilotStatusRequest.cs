namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;

public class SelectPilotStatusRequest
{
    public long ProviderId { get; set; }
    public Guid CacheKey { get; set; }
    public bool IsEdit { get; set; }
}